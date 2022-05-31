using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Data.Overlays;
using NoTankYou.Interfaces;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.PartyFrameOverlayWindow
{
    internal unsafe class PartyFrameOverlayWindow : Window, IDisposable, ICommand
    {
        private readonly Stopwatch AnimationStopwatch = new();
        private PartyOverlaySettings Settings => Service.Configuration.DisplaySettings.PartyOverlay;

        private readonly TextureWrap WarningIcon;

        private delegate bool IsInSanctuary();

        [Signature("E8 ?? ?? ?? ?? 84 C0 75 21 48 8B 4F 10")]
        private readonly IsInSanctuary SanctuaryFunction = null!;

        public PartyFrameOverlayWindow() : base("NoTankYouPartyFrameOverlay")
        {
            SignatureHelper.Initialise(this);

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
            var genericShouldShow = Condition.ShouldShowWindow();
            var enabled = Settings.Enabled;
            var inSanctuary = Settings.DisableInSanctuary && SanctuaryFunction();

            IsOpen = genericShouldShow && enabled && !inSanctuary;

            if (!IsOpen)
            {
                ResetAllAnimation();
            }
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
            Service.HudManager.ForEach(SetWarningForPartyIndex);
        }

        private void SetWarningForPartyIndex(int index, bool targetable, bool visible)
        {
            var updateDictionary = Service.HudManager.WarningStates;
            var warning = updateDictionary[index];

            if (warning != null && targetable && visible)
            {
                AnimateShieldWarning(warning.MessageLong, index);
            }
            else
            {
                ResetAnimation(index, targetable, visible);
            }
        }

        public static void ResetAllAnimation()
        {
            Service.HudManager.ForEach(ResetAnimation);
        }

        private static void ResetAnimation(int hudPartyIndex, bool targetable, bool visible)
        {
            var partyMember = Service.HudManager[hudPartyIndex];

            var hudMember = Service.HudManager.GetHudGroupMember(hudPartyIndex);
            if (hudMember != 0)
            {
                partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(true);
                partyMember.Name->AtkResNode.AddRed = 0;
                partyMember.Name->AtkResNode.AddGreen = 0;
                partyMember.Name->AtkResNode.AddRed = 0;
            }
            else
            {
                partyMember.ClassJobIcon->AtkResNode.ToggleVisibility(false);
                partyMember.Name->AtkResNode.AddRed = 0;
                partyMember.Name->AtkResNode.AddGreen = 0;
                partyMember.Name->AtkResNode.AddRed = 0;
            }
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
