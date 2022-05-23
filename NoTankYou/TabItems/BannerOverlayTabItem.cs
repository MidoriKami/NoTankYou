using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;
using NoTankYou.Windows.PartyFrameOverlayWindow;

namespace NoTankYou.TabItems
{
    internal class BannerOverlayTabItem : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.BannerOverlay;
        public string ConfigurationPaneLabel => Strings.TabItems.BannerOverlay.ConfigurationLabel;
        public string AboutInformationBox => Strings.TabItems.BannerOverlay.Description;
        public string TechnicalInformation => Strings.TabItems.BannerOverlay.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        private static BannerOverlaySettings Settings => Service.Configuration.DisplaySettings.BannerOverlay;

        private readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    if (!Settings.Enabled)
                    {
                        PartyFrameOverlayWindow.ResetAllAnimation();
                    }

                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox ModeSelect = new()
        {
            Label = Strings.Common.Labels.ModeSelect,
            ContentsAction = () =>
            {
                var mode = (int) Settings.Mode;

                var region = ImGui.GetContentRegionAvail() * 0.80f;

                if (ImGui.BeginTable("ModeSelectTable", 2, ImGuiTableFlags.None, new Vector2(region.X, -1)))
                {
                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.TabItems.BannerOverlay.ListMode, ref mode, (int)BannerOverlayDisplayMode.List);
                    ImGuiComponents.HelpMarker(Strings.TabItems.BannerOverlay.ListModeDescription);
                    
                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.TabItems.BannerOverlay.TopPriorityMode, ref mode, (int)BannerOverlayDisplayMode.TopPriority);
                    ImGuiComponents.HelpMarker(Strings.TabItems.BannerOverlay.TopPriorityDescription);

                    ImGui.EndTable();
                }

                if (Settings.Mode != (BannerOverlayDisplayMode) mode)
                {
                    Settings.Mode = (BannerOverlayDisplayMode) mode;
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox ListModeOptions = new()
        {
            Label = Strings.TabItems.BannerOverlay.ListModeOptions,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(50.0f * ImGuiHelpers.GlobalScale);
                ImGui.InputInt(Strings.TabItems.BannerOverlay.WarningCount, ref Settings.WarningCount, 0, 0);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Settings.WarningCount = Math.Clamp(Settings.WarningCount, 1, 8);
                }

                ImGuiComponents.HelpMarker(Strings.TabItems.BannerOverlay.WarningCountDescription);
            }
        };

        private readonly InfoBox ScaleOptions = new()
        {
            Label = Strings.Common.Labels.Scale,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
                ImGui.DragFloat(Strings.Common.Labels.Scale, ref Settings.Scale, 0.01f, 0.1f, 5.0f);

                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Service.Configuration.Save();
                }
            }
        };

        private static float _xCoordinate = Settings.Position.X;
        private static float _yCoordinate = Settings.Position.Y;

        private readonly InfoBox RepositionMode = new()
        {
            Label = Strings.TabItems.BannerOverlay.RepositionMode,
            ContentsAction = () =>
            {

                var region = ImGui.GetContentRegionAvail() * 0.80f;

                if (ImGui.BeginTable("ModeSelectTable", 2, ImGuiTableFlags.None, new Vector2(region.X, -1)))
                {
                    if (Settings.Reposition)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextColored(Colors.Green ,Strings.Common.Labels.Unlocked);

                        ImGui.TableNextColumn();
                        if (ImGui.Button(Strings.Common.Labels.Lock, ImGuiHelpers.ScaledVector2(75.0f, 23.0f)))
                        {
                            Settings.Reposition = false;
                        }
                    }
                    else
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextColored(Colors.SoftRed, Strings.Common.Labels.Locked);

                        ImGui.TableNextColumn();
                        if (ImGui.Button(Strings.Common.Labels.Unlock, ImGuiHelpers.ScaledVector2(75.0f, 23.0f)))
                        {
                            Settings.Reposition = true;
                        }
                    }

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGuiHelpers.ScaledDummy(15.0f);
                    
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                    ImGui.InputFloat("X", ref _xCoordinate, 0, 0, "%.1f");
                    
                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                    ImGui.InputFloat("Y", ref _yCoordinate, 0, 0, "%.1f");

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();

                    if (Settings.Reposition)
                    {
                        ImGui.TextColored(Colors.SoftRed, Strings.TabItems.BannerOverlay.UnlockToSave);
                    }
                    else
                    {
                        if (ImGui.Button(Strings.Common.Labels.Apply, ImGuiHelpers.ScaledVector2(75.0f, 23.0f)))
                        {
                            Settings.Position = new Vector2(_xCoordinate, _yCoordinate);
                            Service.Configuration.Save();
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox DisplayOptions = new()
        {
            Label = Strings.Common.Labels.DisplayOptions,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.TabItems.BannerOverlay.ExclamationMark, ref Settings.WarningShield))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.TabItems.BannerOverlay.WarningText, ref Settings.WarningText))
                {
                    Service.Configuration.Save();
                }

                if (Settings.WarningText)
                {
                    ImGui.Indent(15.0f * ImGuiHelpers.GlobalScale);

                    if (ImGui.Checkbox(Strings.TabItems.BannerOverlay.PlayerNames, ref Settings.PlayerNames))
                    {
                        Service.Configuration.Save();
                    }

                    ImGui.Indent(-15.0f * ImGuiHelpers.GlobalScale);
                }

                if (ImGui.Checkbox(Strings.TabItems.BannerOverlay.Icon, ref Settings.Icon))
                {
                    Service.Configuration.Save();
                }
            }
        };

        public BannerOverlayTabItem()
        {
            AboutImage = Image.LoadImage("BannerOverlay");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.TabItems.BannerOverlay.Label);
        }

        public void DrawOptions()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            ScaleOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            RepositionMode.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            DisplayOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            ModeSelect.DrawCentered();

            if (Settings.Mode == BannerOverlayDisplayMode.List)
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                ListModeOptions.DrawCentered();
            }

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
