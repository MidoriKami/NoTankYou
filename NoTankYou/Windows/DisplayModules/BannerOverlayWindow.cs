using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.DisplayModules
{
    internal class BannerOverlayWindow : Window, IDisposable, ICommand
    {
        private static BannerOverlaySettings Settings => Service.Configuration.DisplaySettings.BannerOverlay;
        private readonly TextureWrap WarningIcon;
        private readonly TextureWrap Crosshair;

        private int WarningsDisplayed;

        private delegate bool IsInSanctuary();

        [Signature("E8 ?? ?? ?? ?? 84 C0 75 21 48 8B 4F 10")]
        private readonly IsInSanctuary SanctuaryFunction = null!;

        public BannerOverlayWindow() : base("NoTankYouBannerOverlay")
        {
            SignatureHelper.Initialise(this);

            Service.WindowSystem.AddWindow(this);

            WarningIcon = Image.LoadImage("BigWarning");
            Crosshair = Image.LoadImage("Crosshair");

            ForceMainWindow = true;

            Flags |= ImGuiWindowFlags.NoDecoration;
            Flags |= ImGuiWindowFlags.NoBackground;
            Flags |= ImGuiWindowFlags.NoInputs;
            Flags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
            Flags |= ImGuiWindowFlags.NoFocusOnAppearing;
            Flags |= ImGuiWindowFlags.NoNavFocus;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            var genericShouldShow = Condition.ShouldShowWarnings();
            var enabled = Settings.Enabled;
            var inSanctuary = Settings.DisableInSanctuary && SanctuaryFunction();

            IsOpen = genericShouldShow && enabled && !inSanctuary;
        }

        public override void PreDraw()
        {
            Size = ImGuiHelpers.ScaledVector2(90.0f, 90.0f);
        }

        public override void Draw()
        {
            LockUnlockWindow();

            WarningsDisplayed = 0;

            Service.HudManager.ForEach(ProcessMember);
        }

        private void ProcessMember(int index, bool targetable, bool visible)
        {
            if (!targetable) return;

            var playerKey = (uint)Service.HudManager.GetHudGroupMember(index);

            var warning = Service.HudManager.WarningStates[index];

            if (warning != null)
            {
                var player = PlayerLocator.GetPlayer(playerKey);

                if (player != null)
                {
                    DrawWarningStatBanner(warning, player);
                }
            }
        }

        private void LockUnlockWindow()
        {
            if (Settings.Reposition)
            {
                Flags &= ~ImGuiWindowFlags.NoInputs;
                DrawCrosshair();
            }
            else
            {
                Flags |= ImGuiWindowFlags.NoInputs;
            }
        }

        private void DrawCrosshair()
        {
            ImGui.Image(Crosshair.ImGuiHandle, ImGuiHelpers.ScaledVector2(75.0f, 75.0f));

            var drawList = ImGui.GetBackgroundDrawList();

            var windowPosition = ImGui.GetWindowPos();

            var xPosition = windowPosition.X + 90.0f / 2.0f * ImGuiHelpers.GlobalScale;
            var yPosition = windowPosition.Y + 90.0f / 2.0f * ImGuiHelpers.GlobalScale;

            var centerPoint = new Vector2(xPosition, yPosition);
            drawList.AddCircleFilled(centerPoint, 2.0f, ImGui.GetColorU32(Colors.Green));

            if (Settings.Position != centerPoint)
            {
                Service.Configuration.Save();
                Settings.Position = centerPoint;
            }
        }

        private void DrawWarningStatBanner(WarningState state, PlayerCharacter player)
        {
            switch (Settings.Mode)
            {
                case BannerOverlayDisplayMode.TopPriority when WarningsDisplayed == 0:
                    DrawBanner(Settings.Position, state.MessageShort, player.Name.TextValue, Settings.Scale, state.IconID, state.IconLabel);
                    WarningsDisplayed = 1;
                    break;

                case BannerOverlayDisplayMode.List when WarningsDisplayed < Settings.WarningCount:
                    var adjustedPosition = Settings.Position + new Vector2(0.0f, 70.0f) * WarningsDisplayed;
                    DrawBanner(adjustedPosition, state.MessageShort, player.Name.TextValue, Settings.Scale, state.IconID, state.IconLabel);
                    WarningsDisplayed += 1;
                    break;
            }
        }

        private void DrawBanner(Vector2 startPosition, string warningText, string source, float scale, uint iconId, string skillName)
        {
            var drawList = ImGui.GetBackgroundDrawList();

            var startingPosition = startPosition;

            if (Settings.WarningShield)
            {
                var imageSize = new Vector2(WarningIcon.Width, WarningIcon.Height) * scale;

                drawList.AddImage(WarningIcon.ImGuiHandle, startingPosition, startingPosition + imageSize);
                startingPosition = startingPosition with { X = startingPosition.X + imageSize.X + 10.0f * scale, Y = startingPosition.Y + 4.0f };
            }

            if (Settings.WarningText)
            {
                DrawUtilities.TextOutlined(startingPosition, warningText, scale);

                var textSize = DrawUtilities.CalculateTextSize(warningText, scale);
                startingPosition = startingPosition with { X = startingPosition.X + textSize.X };

                if (Settings.PlayerNames)
                {
                    textSize = DrawUtilities.CalculateTextSize(source, scale);
                    startingPosition = startingPosition with { X = startingPosition.X - textSize.X / 2.0f, Y = startingPosition.Y + textSize.Y };
                    DrawUtilities.TextOutlined(startingPosition, source, scale / 2.0f);
                    startingPosition = startingPosition with { X = startingPosition.X + textSize.X / 2.0f, Y = startingPosition.Y - textSize.Y };
                }

            }

            if (Settings.Icon)
            {
                DrawUtilities.DrawIconWithName(startingPosition, iconId, skillName, scale, Settings.IconText);
            }
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == "banneroverlay")
            {
                switch (secondaryCommand)
                {
                    case null:
                        Settings.Enabled = !Settings.Enabled;
                        break;

                    case "on":
                        Settings.Enabled = true;
                        break;

                    case "off":
                        Settings.Enabled = false;
                        break;
                }
            }
        }
    }
}
