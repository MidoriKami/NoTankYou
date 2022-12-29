using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class FreeCompanyConfiguration : GenericSettings
{
    public Setting<FreeCompanyBuffScanMode> ScanMode = new(FreeCompanyBuffScanMode.Any);
    public Setting<int> BuffCount = new(1);
    public uint[] BuffList = new uint[2];
}

internal class FreeCompany : IModule
{
    public ModuleName Name => ModuleName.FreeCompany;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static FreeCompanyConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.FreeCompany;
    public GenericSettings GenericSettings => Settings;

    public FreeCompany()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    internal class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Common.Labels.ModeSelect)
                .BeginTable()
                .BeginRow()
                .AddConfigRadio(Strings.Modules.FreeCompany.Any, Settings.ScanMode, FreeCompanyBuffScanMode.Any)
                .AddConfigRadio(Strings.Modules.FreeCompany.Specific, Settings.ScanMode, FreeCompanyBuffScanMode.Specific)
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.FreeCompany.BuffCount)
                .AddAction(DrawBuffCount)
                .Draw();

            if (Settings.ScanMode.Value == FreeCompanyBuffScanMode.Specific)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Modules.FreeCompany.BuffSelection)
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
                            Service.ConfigurationManager.Save();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
        }

        private void DrawBuffCount()
        {
            ImGui.PushItemWidth(200.0f);
            if (ImGui.BeginCombo("###BuffCountCombo", Settings.BuffCount.ToString()))
            {
                if (ImGui.Selectable("1", Settings.BuffCount.Value == 1))
                {
                    Settings.BuffCount.Value = 1;
                    Service.ConfigurationManager.Save();
                }

                if (ImGui.Selectable("2", Settings.BuffCount.Value == 2))
                {
                    Settings.BuffCount.Value = 2;
                    Service.ConfigurationManager.Save();
                }

                ImGui.EndCombo();
            }
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
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

            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            FreeCompanyStatusIDList = Service.DataManager.GetExcelSheet<Status>()!
                .Where(status => status.IsFcBuff)
                .Select(status => status.RowId)
                .ToList();

            FreeCompanyStatus = Service.DataManager.GetExcelSheet<CompanyAction>()!.GetRow(43)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Service.DutyEventManager.DutyStarted) return null;
            if (!CurrentlyInHomeworld(character)) return null;

            switch (Settings.ScanMode.Value)
            {
                case FreeCompanyBuffScanMode.Any:
                    var fcBuffCount = character.StatusCount(FreeCompanyStatusIDList);
                    if (fcBuffCount < Settings.BuffCount.Value)
                    {
                        return new WarningState
                        {
                            MessageLong = Strings.Modules.FreeCompany.WarningText,
                            MessageShort = Strings.Modules.FreeCompany.WarningTextShort,
                            IconID = (uint)FreeCompanyStatus.Icon,
                            IconLabel = "",
                            Priority = Settings.Priority.Value,
                        };
                    }
                    break;

                case FreeCompanyBuffScanMode.Specific:
                    for (var i = 0; i < Settings.BuffCount.Value; ++i)
                    {
                        var targetStatus = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i])!;

                        if (!character.HasStatus(targetStatus.RowId))
                        {
                            return new WarningState
                            {
                                MessageLong = Strings.Modules.FreeCompany.WarningText,
                                MessageShort = Strings.Modules.FreeCompany.WarningTextShort,
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

        private bool CurrentlyInHomeworld(PlayerCharacter character)
        {
            var homeworld = character.HomeWorld.Id;
            var currentWorld = character.CurrentWorld.Id;

            return homeworld == currentWorld;
        }
    }
}