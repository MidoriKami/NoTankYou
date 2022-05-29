using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Data.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Interfaces;
using NoTankYou.System;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.PartyFrameOverlayWindow
{
    internal unsafe class PartyFrameOverlayWindow : Window, IDisposable, ICommand
    {
        private readonly Stopwatch AnimationStopwatch = new();
        private PartyOverlaySettings Settings => Service.Configuration.DisplaySettings.PartyOverlay;
        private static BlacklistSettings BlacklistSettings => Service.Configuration.SystemSettings.Blacklist;

        private readonly TextureWrap WarningIcon;

        public PartyFrameOverlayWindow() : base("NoTankYouPartyFrameOverlay")
        {
            Service.WindowSystem.AddWindow(this);

            WarningIcon = Image.LoadImage("Warning");

            Flags |= ImGuiWindowFlags.NoDecoration;
            Flags |= ImGuiWindowFlags.NoBackground;
            Flags |= ImGuiWindowFlags.NoInputs;
            Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
            Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
            Flags |= ImGuiWindowFlags.NoNavFocus;

            ForceMainWindow = true;

            AnimationStopwatch.Start();
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);

            ResetAllAnimation();
        }
        
        public override void PreOpenCheck()
        {
            var enabled = Settings.Enabled;
            var partyListVisible = HudManager.IsPartyListVisible();
            var isPvP = Territory.IsPvP();
            var blacklisted = BlacklistSettings.Enabled && BlacklistSettings.ContainsCurrentZone();
            var inCrossWorldParty = Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];

            IsOpen = partyListVisible && enabled && !isPvP && !inCrossWorldParty && !blacklisted;
        }

        public override void PreDraw()
        {
            var partyFrameSize = Service.HudManager.GetPartyFrameSize();
            var partyFramePosition = Service.HudManager.GetPartyFramePosition();

            Position = partyFramePosition;
            Size = partyFrameSize;
        }

        public override void Draw()
        {
            if (Service.HudManager.Disabled)
            {
                ResetAllAnimation();
            }
            else
            {
                Service.HudManager.ForEach(memberId =>
                {
                    var playerKey = (uint)Service.HudManager.GetHudGroupMember(memberId);

                    var updateDictionary = Service.HudManager.WarningStates;

                    if (updateDictionary.ContainsKey(playerKey))
                    {
                        AnimateShieldWarning(updateDictionary[playerKey].MessageLong, memberId);
                    }
                    else
                    {
                        ResetAnimation(memberId);
                    }
                });
            }
        }

        public static void ResetAllAnimation()
        {
            Service.HudManager.ForEach(ResetAnimation);
        }

        private static void ResetAnimation(int hudPartyIndex)
        {
            var partyMember = Service.HudManager[hudPartyIndex];
            partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(true);
            partyMember.Name->AtkResNode.AddRed = 0;
        }
        
        private void AnimateShieldWarning(string warningText, int hudPartyIndex)
        {
            var partyMember = Service.HudManager[hudPartyIndex];

            if (Settings.JobIcon)
            {
                AnimateJobIcon(partyMember, hudPartyIndex);
            }

            if (Settings.PlayerName)
            {
                AnimatePlayerName(partyMember);
            }

            if (Settings.WarningText)
            {
                AnimateWarningText(hudPartyIndex, warningText);
            }

            if (AnimationStopwatch.ElapsedMilliseconds >= 1300)
                AnimationStopwatch.Restart();
        }

        private void AnimateJobIcon(AddonPartyList.PartyListMemberStruct partyMember, int hudPartyIndex)
        {
            if (!Settings.FlashingEffects)
            {
                partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(false);
                DrawWarningShield(hudPartyIndex);
            }
            else
            {
                if (AnimationStopwatch.ElapsedMilliseconds < 500)
                {
                    partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(true);
                }
                else if (AnimationStopwatch.ElapsedMilliseconds > 500)
                {
                    partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(false);
                    DrawWarningShield(hudPartyIndex);
                }
            }
        }

        private void AnimatePlayerName(AddonPartyList.PartyListMemberStruct partyMember)
        {
            if (!Settings.FlashingEffects)
            {
                partyMember.Name->AtkResNode.AddRed = (ushort)(Settings.WarningOutlineColor.X * 255);
                partyMember.Name->AtkResNode.AddGreen = (ushort) ( Settings.WarningOutlineColor.Y * 255 );
                partyMember.Name->AtkResNode.AddBlue = (ushort) ( Settings.WarningOutlineColor.Z * 255 );
            }
            else
            {
                if (AnimationStopwatch.ElapsedMilliseconds < 500)
                {
                    partyMember.Name->AtkResNode.AddRed = 0;
                    partyMember.Name->AtkResNode.AddGreen = 0;
                    partyMember.Name->AtkResNode.AddRed = 0;

                }
                else if (AnimationStopwatch.ElapsedMilliseconds > 500)
                {
                    partyMember.Name->AtkResNode.AddRed = (ushort)(Settings.WarningOutlineColor.X * 255);
                    partyMember.Name->AtkResNode.AddGreen = (ushort) ( Settings.WarningOutlineColor.Y * 255 );
                    partyMember.Name->AtkResNode.AddBlue = (ushort) ( Settings.WarningOutlineColor.Z * 255 );
                }
            }
        }

        private void AnimateWarningText(int hudPartyIndex, string warningText)
        {
            if (!Settings.FlashingEffects)
            {
                DrawText(Settings.WarningTextColor, warningText, hudPartyIndex);
            }
            else
            {
                if (AnimationStopwatch.ElapsedMilliseconds < 500)
                {
                    DrawText(Colors.White, warningText, hudPartyIndex);

                }
                else if (AnimationStopwatch.ElapsedMilliseconds > 500)
                {
                    DrawText(Settings.WarningTextColor, warningText, hudPartyIndex);
                }
            }
        }

        private void DrawWarningShield(int hudPartyIndex)
        {
            var iconInfo = Service.HudManager.GetJobLocationInfo(hudPartyIndex);
            var drawPosition = iconInfo.Position + (iconInfo.Size * 0.10f);
            ImGui.SetCursorPos(drawPosition);
            ImGui.Image(WarningIcon.ImGuiHandle, iconInfo.Size * 0.80f);
        }

        private void DrawText(Vector4 color, string warningText, int hudPartyIndex)
        {
            var nameInfo = Service.HudManager.GetNameLocationInfo(hudPartyIndex);
            var textSize = ImGui.CalcTextSize(warningText);

            var warningTextPosition = nameInfo.Position with {X = nameInfo.Position.X + nameInfo.Size.X - textSize.X, Y = nameInfo.Position.Y - 10.0f};
            ImGui.SetCursorPos(warningTextPosition);
            ImGui.TextColored(color, warningText);
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == "partyoverlay")
            {
                switch (secondaryCommand)
                {
                    case null:
                        Settings.Enabled = !Settings.Enabled;
                        ResetAllAnimation();
                        break;

                    case "on":
                        Settings.Enabled = true;
                        ResetAllAnimation();
                        break;

                    case "off":
                        Settings.Enabled = false;
                        ResetAllAnimation();
                        break;
                }
            }
        }
    }
}
