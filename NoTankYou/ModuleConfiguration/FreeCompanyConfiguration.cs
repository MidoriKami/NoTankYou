using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class FreeCompanyConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.FreeCompany;
        public string ConfigurationPaneLabel => Strings.Modules.FreeCompany.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.FreeCompany.Description;
        public string TechnicalInformation => Strings.Modules.FreeCompany.TechnicalDescription;
        public TextureWrap? AboutImage { get; }

        private static FreeCompanyModuleSettings Settings => Service.Configuration.ModuleSettings.FreeCompany;

        private readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox ModeSelect = new()
        {
            Label = Strings.Common.Labels.ModeSelect,
            ContentsAction = () =>
            {
                var mode = (int) Settings.ScanMode;

                var region = ImGui.GetContentRegionAvail() * 0.80f;

                if (ImGui.BeginTable("ModeSelectTable", 2, ImGuiTableFlags.None, new Vector2(region.X, -1)))
                {
                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.Modules.FreeCompany.Any, ref mode, (int)FreeCompanyBuffScanMode.Any);
                    ImGuiComponents.HelpMarker(Strings.Modules.FreeCompany.AnyDescription);

                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.Modules.FreeCompany.Specific, ref mode, (int)FreeCompanyBuffScanMode.Specific);
                    ImGuiComponents.HelpMarker(Strings.Modules.FreeCompany.SpecificDescription);

                    ImGui.EndTable();
                }

                if (Settings.ScanMode != (FreeCompanyBuffScanMode) mode)
                {
                    Settings.ScanMode = (FreeCompanyBuffScanMode) mode;
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox BuffCount = new()
        {
            Label = Strings.Modules.FreeCompany.BuffCount,
            ContentsAction = () =>
            {
                if (ImGui.BeginCombo("###BuffCountCombo", Settings.BuffCount.ToString()))
                {
                    if (ImGui.Selectable("1", Settings.BuffCount == 1))
                    {
                        Settings.BuffCount = 1;
                        Service.Configuration.Save();
                    }

                    if (ImGui.Selectable("2", Settings.BuffCount == 2))
                    {
                        Settings.BuffCount = 2;
                        Service.Configuration.Save();
                    }

                    ImGui.EndCombo();
                }
            }
        };

        private readonly InfoBox BuffSelection = new()
        {
            Label = Strings.Modules.FreeCompany.BuffSelection,
            ContentsAction = () =>
            {
                for (var i = 0; i < Settings.BuffCount; i++)
                {
                    var displayValue = Strings.Common.Labels.Unset;
                    if (Settings.BuffList[i] != 0)
                    {
                        var status = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i]);

                        if (status != null)
                        {
                            displayValue = status.Name.RawString;
                        }
                    }
                
                    if (ImGui.BeginCombo($"###BuffSelection{i}", displayValue))
                    {
                        foreach (var status in Service.DataManager.GetExcelSheet<Status>()!.Where(status => status.IsFcBuff).OrderBy(s => s.Name.RawString))
                        {
                            if (ImGui.Selectable(status.Name.RawString, status.RowId == Settings.BuffList[i]))
                            {
                                Settings.BuffList[i] = status.RowId;
                                Service.Configuration.Save();
                            }
                        }

                        ImGui.EndCombo();
                    }
                }
            }
        };

        private readonly InfoBox Priority = new()
        {
            Label = Strings.Common.Labels.Priority,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                ImGui.InputInt("", ref Settings.Priority, 1, 1);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Settings.Priority = Math.Clamp(Settings.Priority, 0, 10);
                }
            }
        };

        public FreeCompanyConfiguration()
        {
            AboutImage = Image.LoadImage("FreeCompany");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.FreeCompany.Label);
        }

        public void DrawOptions()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            Priority.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            ModeSelect.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            BuffCount.DrawCentered();

            if (Settings.ScanMode == FreeCompanyBuffScanMode.Specific)
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                BuffSelection.DrawCentered();
            }

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
