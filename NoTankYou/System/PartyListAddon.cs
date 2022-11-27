using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace NoTankYou.System;

public readonly unsafe struct PartyListAddonData
{
    public static readonly Dictionary<uint, Stopwatch> TimeSinceLastTargetable = new();

    public AddonPartyList.PartyListMemberStruct UserInterface { get; init; }
    public PartyMemberData AgentData { get; init; }
    public PlayerCharacter? PlayerCharacter { get; init; }

    private bool Targetable => UserInterface.PartyMemberComponent->OwnerNode->AtkResNode.Color.A != 0x99;
    
    public bool IsTargetable()
    {
        if (!AgentData.ValidData) return false;

        TimeSinceLastTargetable.TryAdd(AgentData.ObjectID, Stopwatch.StartNew());
        var stopwatch = TimeSinceLastTargetable[AgentData.ObjectID];
            
        if (Targetable)
        {
            // Returns true if the party member has been targetable for 2second or more
            return stopwatch.Elapsed >= TimeSpan.FromSeconds(2);
        }
        else
        {
            // Returns false, and continually resets the stopwatch
            stopwatch.Restart();
            return false;
        }
    }
}

internal readonly struct PartyFramePositionInfo
{
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }
    public Vector2 Scale { get; init; }

    public override string ToString() => $"{{Position: {Position}, Size: {Size}, Scale: {Scale}}}";
}

internal unsafe class PartyListAddon : IEnumerable<PartyListAddonData>, IDisposable
{
    public IEnumerator<PartyListAddonData> GetEnumerator()
    {
        return AddonData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static AddonPartyList* PartyList => (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList", 1);
    public static bool DataAvailable => PartyList != null && PartyList->AtkUnitBase.RootNode != null;

    private readonly List<PartyListAddonData> AddonData = new();

    public PartyListAddon()
    {
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        AddonData.Clear();
        if (!DataAvailable) return;
        if (PartyList->MemberCount <= 0) return;
        
        foreach (var index in Enumerable.Range(0, PartyList->MemberCount))
        {
            var agentData = HudAgent.GetPartyMember(index);
            var playerCharacter = HudAgent.GetPlayerCharacter(index);
            var userInterface = PartyList->PartyMember[index];

            AddonData.Add(new PartyListAddonData
            {
                AgentData = agentData,
                PlayerCharacter = playerCharacter,
                UserInterface = userInterface,
            });
        }
    }

    public static PartyFramePositionInfo GetPositionInfo()
    {
        // Resource Node (id 9) contains a weird offset for the actual list elements
        var rootNode = PartyList->AtkUnitBase.RootNode;
        var addonBasePosition = new Vector2(rootNode->X, rootNode->Y);
        var scale = new Vector2(rootNode->ScaleX, rootNode->ScaleY);

        var partyListNode = PartyList->AtkUnitBase.GetNodeById(9);
        var partyListPositionOffset = new Vector2(partyListNode->X, partyListNode->Y) * scale;
        var partyListSize = new Vector2(partyListNode->Width, partyListNode->Height);

        return new PartyFramePositionInfo
        {
            Size = partyListSize * scale,
            Position = addonBasePosition + partyListPositionOffset,
            Scale = scale,
        };
    }
}

public static unsafe class PartyListMemberStructExtensions
{
    public static void SetPlayerNameOutlineColor(this AddonPartyList.PartyListMemberStruct data, Vector4 color)
    {
        data.Name->AtkResNode.AddRed = (ushort)(color.X * 255);
        data.Name->AtkResNode.AddGreen = (ushort)(color.Y * 255);
        data.Name->AtkResNode.AddBlue = (ushort)(color.Z * 255);
    }

    public static void SetIconVisibility(this AddonPartyList.PartyListMemberStruct data, bool visible)
    {
        data.ClassJobIcon->AtkResNode.ToggleVisibility(visible);
    }

    public static Vector2 GetIconPosition(this AddonPartyList.PartyListMemberStruct data)
    {
        var parentOffset = data.ClassJobIcon->AtkResNode.ParentNode->Y;
        var jobIcon = data.ClassJobIcon->AtkResNode;

        return new Vector2(jobIcon.X, jobIcon.Y + parentOffset);
    }

    public static Vector2 GetIconSize(this AddonPartyList.PartyListMemberStruct data)
    {
        var jobIcon = data.ClassJobIcon->AtkResNode;

        return new Vector2(jobIcon.Width, jobIcon.Height);
    }

    public static Vector2 GetNamePosition(this AddonPartyList.PartyListMemberStruct data)
    {
        var nameElement = data.NameAndBarsContainer;
        var memberOffset = data.PartyMemberComponent->OwnerNode->AtkResNode.Y;

        return new Vector2(nameElement->X, nameElement->Y + memberOffset);
    }

    public static Vector2 GetNameSize(this AddonPartyList.PartyListMemberStruct data)
    {
        var nameElement = data.NameAndBarsContainer;

        return new Vector2(nameElement->Width, nameElement->Height);
    }
}