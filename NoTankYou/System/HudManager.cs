using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Interfaces;
using NoTankYou.Utilities;

namespace NoTankYou.System
{
    public unsafe class HudManager : IDisposable
    {
        #region Signatures
        private delegate IntPtr AddonOnSetup(IntPtr a1);
        private delegate IntPtr AddonFinalize(IntPtr a1);
        private delegate byte AddonResize(IntPtr a1, IntPtr a2);

        [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 54 41 56 41 57 48 83 EC 30 BA ?? ?? ?? ?? 48 8B F1", DetourName = nameof(PartyListSetup))]
        private readonly Hook<AddonOnSetup>? PartyListSetupHook = null;

        [Signature("48 89 5C 24 ?? 57 48 83 EC 20 0F B7 99 ?? ?? ?? ?? 48 8B F9 E8 ?? ?? ?? ?? 8B D3 41 B0 01 48 8D 88 ?? ?? ?? ?? E8 ?? ?? ?? ?? 45 33 C9 33 D2 48 8B CF 45 8D 41 03", DetourName = nameof(PartyListFinalize))]
        private readonly Hook<AddonFinalize>? PartyListFinalizeHook = null;

        [Signature("E8 ?? ?? ?? ?? 48 8B 47 20 BA ?? ?? ?? ?? B9", DetourName = nameof(PartyListResize))]
        private readonly Hook<AddonResize>? PartyListResizeHook = null;
        #endregion

        private static AddonPartyList* _partyList;

        public HashSet<int> PartyMemberObjectIDList { get; private set; }= new();
        public HashSet<PlayerCharacter> PartyMemberPlayerCharacterList { get; private set; } = new();
        public float UIScale { get; private set; }
        public bool Disabled { get; private set; }
        private BlacklistSettings BlacklistSettings => Service.Configuration.SystemSettings.Blacklist;

        private Queue<PlayerCharacter> PlayerUpdateQueue = new();

        private readonly Stopwatch UpdateStopwatch = new();

        public Dictionary<uint, WarningState> WarningStates { get; private set; }= new();

        public HudManager()
        {
            SignatureHelper.Initialise(this);

            _partyList = (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList", 1);
            
            RecalculatePartyMembers();

            UpdateStopwatch.Start();

            PartyListSetupHook?.Enable();
            PartyListFinalizeHook?.Enable();
            PartyListResizeHook?.Enable();

            Service.Framework.Update += FrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;

            PartyListSetupHook?.Dispose();
            PartyListFinalizeHook?.Dispose();
            PartyListResizeHook?.Dispose();
        }

        #region HookedFunctions
        private IntPtr PartyListSetup(IntPtr a1)
        {
            _partyList = (AddonPartyList*) a1;

            return PartyListSetupHook!.Original(a1);
        }

        private IntPtr PartyListFinalize(IntPtr a1)
        {
            _partyList = null;

            return PartyListFinalizeHook!.Original(a1);
        }

        private byte PartyListResize(IntPtr a1, IntPtr a2)
        {
            var result = PartyListResizeHook!.Original(a1, a2);

            RecalculatePartyMembers();

            return result;
        }
        #endregion

        private void FrameworkUpdate(Framework framework)
        {
            Disabled = true;

            if (PlayerUpdateQueue.Count == 0) return;
            if (!IsPartyListVisible()) return;
            if (BlacklistSettings.Enabled && BlacklistSettings.ContainsCurrentZone() || Service.ClientState.IsPvP) return;

            Disabled = false;

            var currentPlayer = PlayerUpdateQueue.Dequeue();

            UpdatePlayer(currentPlayer);

            PlayerUpdateQueue.Enqueue(currentPlayer);
        }

        private void UpdatePlayer(PlayerCharacter character)
        {
            // Reset the warnings for this specific player
            WarningStates.Remove(character.ObjectId);

            // Get all relevant modules to this player
            foreach (var module in Service.ModuleManager.GetModulesForClassJob(character.ClassJob.Id))
            {
                // Evaluate all warnings
                ProcessModule(module, character);
            }
        }

        private void ProcessModule(IModule module, PlayerCharacter character)
        {
            // If this is a valid warning
            if (!module.ShouldShowWarning(character)) return;

            // If we already have an entry
            if (WarningStates.ContainsKey(character.ObjectId))
            {
                var existingEntry = WarningStates[character.ObjectId];

                if (module.GenericSettings.Priority > existingEntry.Priority)
                {
                    WarningStates[character.ObjectId] = module.GetWarningState();
                }
            }

            // Else we need to make a new entry
            else
            {
                WarningStates[character.ObjectId] = module.GetWarningState();
            }
        }

        private void RecalculatePartyMembers()
        {
            if (_partyList != null && _partyList->MemberCount != PartyMemberObjectIDList.Count)
            {
                PartyMemberObjectIDList = GetPartyObjectIDs();
                PartyMemberPlayerCharacterList = GetPlayerCharacters(PartyMemberObjectIDList);

                PlayerUpdateQueue = new Queue<PlayerCharacter>(PartyMemberPlayerCharacterList);
            }
        }

        public int GetHudGroupMember(int index)
        {
            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var baseAddress = (byte*) frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
            const int groupDataOffset = 0xCC8;

            var objectId = *(int*) (baseAddress + groupDataOffset + index * 0x20 + 0x18);

            return objectId;
        }
        
        public bool IsTargetable(uint objectID)
        {
            for (var i = 0; i < _partyList->MemberCount; ++i)
            {
                var partyListMember = _partyList->PartyMember[i];
                var partyListMemberResourceNode = partyListMember.PartyMemberComponent->OwnerNode->AtkResNode;

                // If this element is our target player
                if (GetHudGroupMember(i) == objectID)
                {
                    // Filter by visibility
                    if (!partyListMemberResourceNode.IsVisible) return false;

                    // Filter on target-ability
                    // If alpha is 0x99, partyListMember is out of range
                    if (partyListMemberResourceNode.Color.A == 0x99) return false;

                    return true;
                }
            }

            return false;
        }

        private static HashSet<PlayerCharacter> GetPlayerCharacters(IEnumerable<int> partyMemberObjectIDs)
        {
            return partyMemberObjectIDs
                .Select(PlayerLocator.GetPlayer)
                .Where(player => player != null)
                .ToHashSet()!;
        }

        public static bool IsPartyListVisible()
        {
            return _partyList != null && _partyList->AtkUnitBase.IsVisible;
        }

        private HashSet<int> GetPartyObjectIDs()
        {
            HashSet<int> uiMembers = new();

            for (var i = 0; i < _partyList->MemberCount; ++i)
            {
                uiMembers.Add(GetHudGroupMember(i));
            }

            return uiMembers;
        }

        public AddonPartyList.PartyListMemberStruct this[int index] => _partyList->PartyMember[index];

        public void ForEach(Action<int> partyMemberAction)
        {
            if (!IsPartyListVisible()) return;

            for (var i = 0; i < _partyList->MemberCount; ++i)
            {
                if (_partyList->PartyMember[i].PartyMemberComponent->OwnerNode->AtkResNode.IsVisible)
                {
                    partyMemberAction(i);
                }
            }
        }

        public Vector2 GetPartyFramePosition()
        {
            var resourceNode = _partyList->AtkUnitBase.GetNodeById(8);
            var rootNode = _partyList->AtkUnitBase.RootNode;

            var xOffset = rootNode->X;
            var yOffset = rootNode->Y;

            UIScale = rootNode->ScaleX;

            var xPosition = resourceNode->X * UIScale;
            var yPosition = resourceNode->Y * UIScale;

            return new Vector2(xPosition + xOffset, yPosition + yOffset);
        }

        public Vector2 GetPartyFrameSize()
        {
            var resourceNode = _partyList->AtkUnitBase.GetNodeById(8);
            var width = resourceNode->Width;
            var height = resourceNode->Height;

            return new Vector2(width, height) * UIScale;
        }

        public UILocationInfo GetJobLocationInfo(int partyIndex)
        {
            var jobIcon = _partyList->PartyMember[partyIndex].ClassJobIcon->AtkResNode;
            var memberOffset = _partyList->PartyMember[partyIndex].PartyMemberComponent->OwnerNode->AtkResNode.Y;

            var xPosition = jobIcon.X;
            var yPosition = jobIcon.Y + memberOffset;

            var width = jobIcon.Width;
            var height = jobIcon.Height;

            return new UILocationInfo
            {
                Position = new Vector2(xPosition, yPosition) * UIScale,
                Size = new Vector2(width, height) * UIScale
            };
        }

        public UILocationInfo GetNameLocationInfo(int partyIndex)
        {
            var jobIcon = _partyList->PartyMember[partyIndex].NameAndBarsContainer;
            var memberOffset = _partyList->PartyMember[partyIndex].PartyMemberComponent->OwnerNode->AtkResNode.Y;

            var xPosition = jobIcon->X;
            var yPosition = jobIcon->Y + memberOffset;

            var width = jobIcon->Width;
            var height = jobIcon->Height;

            return new UILocationInfo
            {
                Position = new Vector2(xPosition, yPosition) * UIScale,
                Size = new Vector2(width, height) * UIScale
            };
        }
    }
}
