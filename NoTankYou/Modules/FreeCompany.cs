using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class FreeCompanyConfiguration : GenericSettings
{
    public Setting<FreeCompanyBuffScanMode> ScanMode = new(FreeCompanyBuffScanMode.Any);
    public Setting<int> BuffCount = new(1);
    public uint[] BuffList = new uint[2];
}

public class FreeCompany : IModule
{
    public ModuleName Name => ModuleName.FreeCompany;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static FreeCompanyConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.FreeCompany;
    public GenericSettings GenericSettings => Settings;

    public FreeCompany()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public ISelectable Selectable { get; }
        public ModuleConfigurationComponent(IModule parentModule)
        {
            Selectable = new ConfigurationSelectable(parentModule, this);
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Labels_ModeSelect)
                .BeginTable()
                .BeginRow()
                .AddConfigRadio(Strings.FreeCompany_Any, Settings.ScanMode, FreeCompanyBuffScanMode.Any)
                .AddConfigRadio(Strings.FreeCompany_Specific, Settings.ScanMode, FreeCompanyBuffScanMode.Specific)
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.FreeCompany_BuffCount)
                .AddAction(DrawBuffCount)
                .Draw();

            if (Settings.ScanMode == FreeCompanyBuffScanMode.Specific)
            {
                InfoBox.Instance
                    .AddTitle(Strings.FreeCompany_BuffSelection)
                    .AddAction(BuffSelection)
                    .Draw();
            }

            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }

        private void BuffSelection()
        {
            for (var i = 0; i < Settings.BuffCount.Value; i++)
            {
                var displayValue = Strings.Labels_Unset;
                if (Settings.BuffList[i] != 0)
                {
                    var status = LuminaCache<Status>.Instance.GetRow(Settings.BuffList[i]);

                    if (status != null)
                    {
                        displayValue = status.Name.RawString;
                    }
                }
                
                ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
                if (ImGui.BeginCombo($"###BuffSelection{i}", displayValue))
                {
                    foreach (var status in LuminaCache<Status>.Instance.Where(status => status.IsFcBuff).OrderBy(s => s.Name.RawString))
                    {
                        if (ImGui.Selectable(status.Name.RawString, status.RowId == Settings.BuffList[i]))
                        {
                            Settings.BuffList[i] = status.RowId;
                            Service.ConfigurationManager.Save();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
        }

        private void DrawBuffCount()
        {
            ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
            if (ImGui.BeginCombo("###BuffCountCombo", Settings.BuffCount.ToString()))
            {
                if (ImGui.Selectable("1", Settings.BuffCount == 1))
                {
                    Settings.BuffCount.Value = 1;
                    Service.ConfigurationManager.Save();
                }

                if (ImGui.Selectable("2", Settings.BuffCount == 2))
                {
                    Settings.BuffCount.Value = 2;
                    Service.ConfigurationManager.Save();
                }

                ImGui.EndCombo();
            }
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private readonly List<uint> FreeCompanyStatusIDList;

        private readonly CompanyAction FreeCompanyStatus;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            Settings.SoloMode.Value = true;
            Settings.DutiesOnly.Value = false;

            ClassJobs = LuminaCache<ClassJob>.Instance
                .Select(r => r.RowId)
                .ToList();

            FreeCompanyStatusIDList = LuminaCache<Status>.Instance
                .Where(status => status.IsFcBuff)
                .Select(status => status.RowId)
                .ToList();

            FreeCompanyStatus = LuminaCache<CompanyAction>.Instance.GetRow(43)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (DutyState.Instance.IsDutyStarted) return null;
            if (!CurrentlyInHomeworld(character)) return null;

            switch (Settings.ScanMode.Value)
            {
                case FreeCompanyBuffScanMode.Any:
                    var fcBuffCount = character.StatusCount(FreeCompanyStatusIDList);
                    if (fcBuffCount < Settings.BuffCount.Value)
                    {
                        return new WarningState
                        {
                            MessageLong = Strings.FreeCompany_WarningText,
                            MessageShort = Strings.FreeCompany_WarningTextShort,
                            IconID = (uint)FreeCompanyStatus.Icon,
                            IconLabel = "",
                            Priority = Settings.Priority.Value,
                        };
                    }
                    break;

                case FreeCompanyBuffScanMode.Specific:
                    for (var i = 0; i < Settings.BuffCount.Value; ++i)
                    {
                        var targetStatus = LuminaCache<Status>.Instance.GetRow(Settings.BuffList[i])!;

                        if (!character.HasStatus(targetStatus.RowId))
                        {
                            return new WarningState
                            {
                                MessageLong = Strings.FreeCompany_WarningText,
                                MessageShort = Strings.FreeCompany_WarningTextShort,
                                IconID = (uint)FreeCompanyStatus.Icon,
                                IconLabel = targetStatus.Name.RawString,
                                Priority = Settings.Priority.Value,
                            };
                        }
                    }
                    break;
            }

            return null;
        }

        private static bool CurrentlyInHomeworld(PlayerCharacter character)
        {
            var homeworld = character.HomeWorld.Id;
            var currentWorld = character.CurrentWorld.Id;

            return homeworld == currentWorld;
        }
    }
}