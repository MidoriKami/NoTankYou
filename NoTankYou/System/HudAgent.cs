using System;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace NoTankYou.System;

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
public readonly struct PartyMemberData
{
    [FieldOffset(0x10)] public readonly int LocalContentID;
    [FieldOffset(0x18)] public readonly uint ObjectID;

    public bool ValidData => ObjectID is not 0 and not 0xE000_0000;

    public override string ToString() => $"{{Valid: {ValidData}, LocalID: {LocalContentID:X8}, ObjectID: {ObjectID:X8}}}";
}

[StructLayout(LayoutKind.Explicit, Size = 0x100)]
internal readonly struct PartyListData
{
    [FieldOffset(0x00)] private readonly PartyMemberData PartyMember_0;
    [FieldOffset(0x20)] private readonly PartyMemberData PartyMember_1;
    [FieldOffset(0x40)] private readonly PartyMemberData PartyMember_2;
    [FieldOffset(0x60)] private readonly PartyMemberData PartyMember_3;
    [FieldOffset(0x80)] private readonly PartyMemberData PartyMember_4;
    [FieldOffset(0xA0)] private readonly PartyMemberData PartyMember_5;
    [FieldOffset(0xC0)] private readonly PartyMemberData PartyMember_6;
    [FieldOffset(0xE0)] private readonly PartyMemberData PartyMember_7;

    public PartyMemberData this[int index] =>
        index switch
        {
            0 => PartyMember_0,
            1 => PartyMember_1,
            2 => PartyMember_2,
            3 => PartyMember_3,
            4 => PartyMember_4,
            5 => PartyMember_5,
            6 => PartyMember_6,
            7 => PartyMember_7,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
}

internal static unsafe class HudAgent
{
    private static readonly AgentInterface* HudAgentInterface = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
    private static PartyListData PartyListData => *(PartyListData*)((byte*)HudAgentInterface + 0xCC8);
    private static uint* AllianceMemberObjectID => (uint*) ((byte*)HudAgentInterface + 0xE14);
    
    public static PartyMemberData GetPartyMember(int index) => PartyListData[index];
    public static PlayerCharacter? GetPlayerCharacter(int index) => PlayerLocator.GetPlayer(GetPartyMember(index).ObjectID);
    public static PlayerCharacter? GetAllianceMember(int index) => PlayerLocator.GetPlayer(AllianceMemberObjectID[index]);
}

internal static class PlayerLocator
{
    public static PlayerCharacter? GetPlayer(uint objectId)
    {
        var result = Service.ObjectTable.SearchById(objectId);

        if (result?.GetType() == typeof(PlayerCharacter))
            return result as PlayerCharacter;

        return null;
    }
}