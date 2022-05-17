using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.PartyFrameOverlayWindow
{
    internal unsafe class PartyFrameOverlayWindow : Window, IDisposable
    {
        private readonly Stopwatch AnimationStopwatch = new();

        private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);

        [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
        private readonly Hook<DutyEventDelegate>? DutyEventHook = null;

        private PartyOverlaySettings Settings => Service.Configuration.DisplaySettings.PartyOverlay;

        private AddonPartyList* PartyList => (AddonPartyList*) Service.GameGui.GetAddonByName("_PartyList", 1);

        private readonly TextureWrap WarningIcon;

        private readonly Dictionary<uint, WarningState> UpdateDictionary = new();

        private bool DutyStarted;

        public PartyFrameOverlayWindow() : base("NoTankYouPartyFrameOverlay")
        {
            Service.WindowSystem.AddWindow(this);

            SignatureHelper.Initialise(this);

            WarningIcon = Image.LoadImage("Warning");

            Flags |= ImGuiWindowFlags.NoDecoration;
            Flags |= ImGuiWindowFlags.NoBackground;
            Flags |= ImGuiWindowFlags.NoInputs;

            AnimationStopwatch.Start();

            DutyEventHook?.Enable();

            DutyStarted = true;

            Service.Framework.Update += FrameworkUpdate;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);

            ResetAllAnimation();

            DutyEventHook?.Dispose();

            Service.Framework.Update -= FrameworkUpdate;
        }

        private void FrameworkUpdate(Framework framework)
        {
            if (!Settings.Enabled) return;
            if (!DutyStarted) return;
            if (ImGui.GetFrameCount() % 2 == 0) return;
            if (!IsPartyListVisible()) return;

            // Clear all Updates from last Update
            UpdateDictionary.Clear();

            // 1. Get a list of object ID's for players
            var partyMemberObjectIDs = GetPartyObjectIDs();

            // 2. Get list of PlayerCharacters
            var playerCharacter = GetPlayerCharacters(partyMemberObjectIDs);

            UpdatePlayerStatus(playerCharacter);
        }

        private byte DutyEventFunction(void* a1, void* a2, ushort* a3)
        {
            var category = *(a3);
            var type = *(uint*)(a3 + 4);

            // DirectorUpdate Category
            if (category == 0x6D)
            {
                DutyStarted = type switch
                {
                    // Duty Commenced
                    0x40000001 => true,

                    // Party Wipe
                    0x40000005 => false,

                    // Duty Recommence
                    0x40000006 => true,

                    // Duty Completed
                    0x40000003 => false,
                    _ => DutyStarted
                };
            }

            return DutyEventHook!.Original(a1, a2, a3);
        }

        private HashSet<int> GetPartyObjectIDs()
        {
            HashSet<int> uiMembers = new();

            for (var i = 0; i < PartyList->MemberCount; ++i)
            {
                var partyListMember = PartyList->PartyMember[i];
                var partyListMemberResourceNode = partyListMember.PartyMemberComponent->OwnerNode->AtkResNode;

                // Filter by visibility
                if(!partyListMemberResourceNode.IsVisible) continue;

                // Filter on target-ability
                // If alpha is 0x99, partyListMember is out of range
                if (partyListMemberResourceNode.Color.A == 0x99) continue;
                
                var playerInfo = GetHudGroupMember(i);
                uiMembers.Add(playerInfo.ObjectId);
            }

            return uiMembers;
        }

        private static List<PlayerCharacter> GetPlayerCharacters(IEnumerable<int> partyMemberObjectIDs)
        {
            return partyMemberObjectIDs
                .Select(PlayerLocator.GetPlayer)
                .Where(player => player != null)
                .ToList()!;
        }

        private void UpdatePlayerStatus(List<PlayerCharacter> playerCharacters)
        {
            foreach (var player in playerCharacters)
            {
                // 3. Get ClassJob
                var classJob = player.ClassJob.GameData;
                if (classJob == null) continue;

                // 4. GetModule
                foreach (var module in Service.ModuleManager.GetModulesForClassJob(classJob))
                {
                    ProcessModule(module, player);
                }
            }
        }

        private void ProcessModule(IModule module, PlayerCharacter character)
        {
            // If this is a valid warning
            if (module.ShouldShowWarning(character))
            {
                // If we already have an entry
                if (UpdateDictionary.ContainsKey(character.ObjectId))
                {
                    var existingEntry = UpdateDictionary[character.ObjectId];

                    if (module.GenericSettings.Priority > existingEntry.Priority)
                    {
                        UpdateDictionary[character.ObjectId] = module.GetWarningState();
                    }
                }

                // Else we need to make a new entry
                else
                {
                    UpdateDictionary[character.ObjectId] = module.GetWarningState();
                }
            }
        }

        
        public override void PreOpenCheck()
        {
            var enabled = Settings.Enabled;
            var partyListActive = PartyList != null;
            var partyListVisible = IsPartyListVisible();

            IsOpen = partyListActive && partyListVisible && enabled;
        }

        public override void PreDraw()
        {
            var partyFrameSize = GetPartyFrameSize();
            var partyFramePosition = GetPartyFramePosition();

            Position = partyFramePosition;
            Size = partyFrameSize;
        }

        public override void Draw()
        {
            if (!DutyStarted) return;
            if (!IsPartyListVisible()) return;

            for (var i = 0; i < PartyList->MemberCount; ++i)
            {
                if (PartyList->PartyMember[i].PartyMemberComponent->OwnerNode->AtkResNode.IsVisible)
                {
                    var playerInfo = GetHudGroupMember(i);
                    var playerKey = (uint)playerInfo.ObjectId;

                    if (UpdateDictionary.ContainsKey(playerKey))
                    {
                        AnimateShieldWarning(UpdateDictionary[playerKey].Message, i);
                    }
                    else
                    {
                        ResetAnimation(i);
                    }
                }
            }
        }

        public void ResetAllAnimation()
        {
            if (PartyList != null)
            {
                for (var i = 0; i < PartyList->MemberCount; ++i)
                {
                    ResetAnimation(i);
                }
            }
        }

        private void ResetAnimation(int hudPartyIndex)
        {
            var partyMember = PartyList->PartyMember[hudPartyIndex];
            partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(true);
            partyMember.Name->AtkResNode.AddRed = 0;
        }
        
        private void AnimateShieldWarning(string warningText, int hudPartyIndex)
        {
            // if time [ 0 -> 500 ] (Fade Out as 0 -> 500)
            if (AnimationStopwatch.ElapsedMilliseconds < 500)
            {
                var partyMember = PartyList->PartyMember[hudPartyIndex];

                partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(true);
                partyMember.Name->AtkResNode.AddRed = 0;

                DrawText(Colors.White, warningText, hudPartyIndex);
            }
            else if (AnimationStopwatch.ElapsedMilliseconds > 500)
            {
                var partyMember = PartyList->PartyMember[hudPartyIndex];
                partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(false);
                partyMember.Name->AtkResNode.AddRed = 255;

                DrawWarningShield(hudPartyIndex);
                DrawText(Colors.SoftRed, warningText, hudPartyIndex);

                if (AnimationStopwatch.ElapsedMilliseconds >= 1300)
                    AnimationStopwatch.Restart();
            }
        }

        private void DrawWarningShield(int hudPartyIndex)
        {
            var iconInfo = GetJobLocationInfo(hudPartyIndex);
            var drawPosition = iconInfo.Position + (iconInfo.Size * 0.10f);
            ImGui.SetCursorPos(drawPosition);
            ImGui.Image(WarningIcon.ImGuiHandle, iconInfo.Size * 0.80f);
        }

        private void DrawText(Vector4 color, string warningText, int hudPartyIndex)
        {
            var nameInfo = GetNameLocationInfo(hudPartyIndex);
            var textSize = ImGui.CalcTextSize(warningText);

            var warningTextPosition = nameInfo.Position with {X = nameInfo.Position.X + nameInfo.Size.X - textSize.X, Y = nameInfo.Position.Y - 10.0f};
            ImGui.SetCursorPos(warningTextPosition);
            ImGui.TextColored(color, warningText);
        }

        private Vector2 GetPartyFramePosition()
        {
            var resourceNode = PartyList->AtkUnitBase.GetNodeById(8);
            var rootNode = PartyList->AtkUnitBase.RootNode;

            var xOffset = rootNode->X;
            var yOffset = rootNode->Y;

            var xPosition = resourceNode->X;
            var yPosition = resourceNode->Y;

            return new Vector2(xPosition + xOffset, yPosition + yOffset);
        }

        private Vector2 GetPartyFrameSize()
        {
            var resourceNode = PartyList->AtkUnitBase.GetNodeById(8);
            var width = resourceNode->Width;
            var height = resourceNode->Height;

            return new Vector2(width, height);
        }

        private UILocationInfo GetJobLocationInfo(int partyIndex)
        {
            var jobIcon = PartyList->PartyMember[partyIndex].ClassJobIcon->AtkResNode;
            var memberOffset = PartyList->PartyMember[partyIndex].PartyMemberComponent->OwnerNode->AtkResNode.Y;

            var xPosition = jobIcon.X;
            var yPosition = jobIcon.Y + memberOffset;

            var width = jobIcon.Width;
            var height = jobIcon.Height;

            return new UILocationInfo
            {
                Position = new Vector2(xPosition, yPosition),
                Size = new Vector2(width, height)
            };
        }

        private UILocationInfo GetNameLocationInfo(int partyIndex)
        {
            var jobIcon = PartyList->PartyMember[partyIndex].NameAndBarsContainer;
            var memberOffset = PartyList->PartyMember[partyIndex].PartyMemberComponent->OwnerNode->AtkResNode.Y;

            var xPosition = jobIcon->X;
            var yPosition = jobIcon->Y + memberOffset;

            var width = jobIcon->Width;
            var height = jobIcon->Height;

            return new UILocationInfo
            {
                Position = new Vector2(xPosition, yPosition),
                Size = new Vector2(width, height)
            };
        }

        private bool IsPartyListVisible()
        {
            if (PartyList == null) return false;
            if(PartyList->AtkUnitBase.RootNode == null) return false;

            return PartyList->AtkUnitBase.RootNode->IsVisible;
        }

        private HudGroupMember GetHudGroupMember(int index)
        {
            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var baseAddress = (byte*) frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
            const int groupDataOffset = 0xCC8;

            var contentId = *(ulong*) (baseAddress + groupDataOffset + index * 0x20 + 0x10);
            var objectId = *(int*) (baseAddress + groupDataOffset + index * 0x20 + 0x18);

            return new HudGroupMember
            {
                ContentId = contentId,
                ObjectId = objectId
            };
        }
    }
}
