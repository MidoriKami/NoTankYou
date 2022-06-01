using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using NoTankYou.Components;
using NoTankYou.Utilities;

namespace NoTankYou.System
{
    public unsafe class HudManager : IDisposable
    {
        #region Signatures
        private delegate IntPtr AddonOnSetup(IntPtr a1);
        private delegate IntPtr AddonFinalize(IntPtr a1);

        [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 54 41 56 41 57 48 83 EC 30 BA ?? ?? ?? ?? 48 8B F1", DetourName = nameof(PartyListSetup))]
        private readonly Hook<AddonOnSetup>? PartyListSetupHook = null;

        [Signature("48 89 5C 24 ?? 57 48 83 EC 20 0F B7 99 ?? ?? ?? ?? 48 8B F9 E8 ?? ?? ?? ?? 8B D3 41 B0 01 48 8D 88 ?? ?? ?? ?? E8 ?? ?? ?? ?? 45 33 C9 33 D2 48 8B CF 45 8D 41 03", DetourName = nameof(PartyListFinalize))]
        private readonly Hook<AddonFinalize>? PartyListFinalizeHook = null;
        #endregion

        private static AddonPartyList* _partyList;
        public bool DataAvailable => _partyList != null;
        private float UIScale { get; set; }
        private int CurrentPartyMember { get; set; }

        public readonly WarningState?[] WarningStates = new WarningState?[8];

        public HudManager()
        {
            SignatureHelper.Initialise(this);

            _partyList = (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList", 1);

            PartyListSetupHook?.Enable();
            PartyListFinalizeHook?.Enable();

            Service.Framework.Update += FrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;

            PartyListSetupHook?.Dispose();
            PartyListFinalizeHook?.Dispose();
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

        #endregion

        private void FrameworkUpdate(Framework framework)
        {
            if (!Condition.ShouldShowWindow()) return;
            
            ProcessPartyMember(CurrentPartyMember);

            SelectNextPartyMember();
        }

        private void SelectNextPartyMember()
        {
            CurrentPartyMember += 1;

            if (CurrentPartyMember >= _partyList->MemberCount)
            {
                CurrentPartyMember = 0;
            }
        }

        private void ProcessPartyMember(int currentPartyMember)
        {
            var currentMemberObjectID = (uint)GetHudGroupMember(CurrentPartyMember);

            if (currentMemberObjectID is 0 or 0xE000_0000)
            {
                WarningStates[currentPartyMember] = null;
            }
            else
            {
                WarningStates[currentPartyMember] = EvaluatePartyMember(currentMemberObjectID);
            }
        }

        private WarningState? EvaluatePartyMember(uint playerObjectID)
        {
            var player = PlayerLocator.GetPlayer(playerObjectID);
            if (player == null) return null;

            var highestPriorityWarning =
                Service.ModuleManager.GetModulesForClassJob(player.ClassJob.Id)
                    .Select(module => module.ShouldShowWarning(player))
                    .Where(module => module != null)
                    .DefaultIfEmpty(null)
                    .Aggregate((i1, i2) => i1!.Priority > i2!.Priority ? i1 : i2);

            return highestPriorityWarning;
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
                    // Filter on target-ability
                    // If alpha is 0x99, partyListMember is out of range
                    if (partyListMemberResourceNode.Color.A == 0x99) return false;

                    return true;
                }
            }

            return false;
        }
        
        public AddonPartyList.PartyListMemberStruct this[int index] => _partyList->PartyMember[index];

        public void ForEach(Action<int, bool, bool> action)
        {
            if (_partyList == null) return;

            for (var i = 0; i < _partyList->MemberCount; ++i)
            {
                var memberVisible = _partyList->PartyMember[i].PartyMemberComponent->OwnerNode->AtkResNode.IsVisible;
                var memberTargetable = IsTargetable((uint) GetHudGroupMember(i));

                action(i, memberTargetable, memberVisible);
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
