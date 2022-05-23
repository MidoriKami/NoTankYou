using System;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.System;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.BannerOverlayWindow
{
   internal class BannerOverlayWindow : Window, IDisposable, ICommand
   {
        private static BannerOverlaySettings Settings => Service.Configuration.DisplaySettings.BannerOverlay;
        private readonly TextureWrap WarningIcon;
        private readonly TextureWrap Crosshair;

        private int WarningsDisplayed;

        public BannerOverlayWindow() : base("NoTankYouBannerOverlay")
        {
            Service.WindowSystem.AddWindow(this);

            WarningIcon = Image.LoadImage("BigWarning");
            Crosshair = Image.LoadImage("Crosshair");

            ForceMainWindow = true;

            Flags |= ImGuiWindowFlags.NoDecoration;
            Flags |= ImGuiWindowFlags.NoBackground;
            Flags |= ImGuiWindowFlags.NoInputs;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            var enabled = Settings.Enabled;
            var partyListVisible = HudManager.IsPartyListVisible();
            var isPvP = Service.ClientState.IsPvP;
            var inCrossWorldParty = Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];

            IsOpen = partyListVisible && enabled && !isPvP && !inCrossWorldParty;
        }

        public override void PreDraw()
        {
            Size = ImGuiHelpers.ScaledVector2(90.0f, 90.0f);
        }

        public override void Draw()
        {
            if (Settings.Reposition)
            {
                Flags &= ~ImGuiWindowFlags.NoInputs;
                DrawCrosshair();
            }

            WarningsDisplayed = 0;

            Service.HudManager.ForEach(memberId =>
            {
                var playerKey = (uint)Service.HudManager.GetHudGroupMember(memberId);

                var updateDictionary = Service.HudManager.WarningStates;

                if (updateDictionary.ContainsKey(playerKey))
                {
                    var warning = updateDictionary[playerKey];
                    var player = PlayerLocator.GetPlayer((int)playerKey);

                    if (player != null)
                    {
                        DrawWarningStatBanner(warning, player);
                    }
                }
            });
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
                startingPosition = startingPosition with{ X = startingPosition.X + imageSize.X + 10.0f * scale, Y = startingPosition.Y + 4.0f };
            }

            if (Settings.WarningText)
            {
                DrawUtilities.TextOutlined(startingPosition, warningText, scale);

                var textSize = DrawUtilities.CalculateTextSize(warningText, scale);
                startingPosition = startingPosition with {X = startingPosition.X + textSize.X};

                if (Settings.PlayerNames)
                {
                    textSize = DrawUtilities.CalculateTextSize(source, scale);
                    startingPosition = startingPosition with {X = startingPosition.X - textSize.X / 2.0f, Y = startingPosition.Y + textSize.Y};
                    DrawUtilities.TextOutlined(startingPosition, source, scale / 2.0f);
                    startingPosition = startingPosition with { X = startingPosition.X + textSize.X / 2.0f, Y = startingPosition.Y - textSize.Y };
                }

            }

            if (Settings.Icon)
            {
                DrawUtilities.DrawIconWithName(startingPosition, iconId, skillName, scale);
            }
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == "partyoverlay")
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
