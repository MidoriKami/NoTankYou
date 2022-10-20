using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;

namespace NoTankYou.Modules;

public class SpiritBondConfiguration : GenericSettings 
{
    public Setting<int> SpiritBondEarlyWarningTime = new(600);
    public Setting<bool> SavageDuties = new(false);
    public Setting<bool> UltimateDuties = new(false);
    public Setting<bool> ExtremeUnreal = new(false);
    public Setting<bool> DisableInCombat = new(false);
    public Setting<bool> CriterionDuties = new(false);
}

internal class Spiritbond : IModule
{
    public ModuleName Name => ModuleName.Spiritbond;
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    internal static SpiritBondConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.SpiritBond;
    public GenericSettings GenericSettings => Settings;

    public Spiritbond()
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
            InfoBox.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Modules.SpiritBond.EarlyWarningLabel)
                .AddInputInt(Strings.Common.Labels.Seconds, Settings.SpiritBondEarlyWarningTime, 0, 3600, 0, 0, 100.0f)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.SpiritBond.AdditionalOptionsLabel)
                .AddConfigCheckbox(Strings.Modules.SpiritBond.SuppressInCombat, Settings.DisableInCombat)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.SpiritBond.ZoneFilters)
                .AddString(Strings.Modules.SpiritBond.ZoneFiltersDescription)
                .AddConfigCheckbox(Strings.Common.Labels.Savage, Settings.SavageDuties)
                .AddConfigCheckbox(Strings.Common.Labels.Ultimate, Settings.UltimateDuties)
                .AddConfigCheckbox(Strings.Common.Labels.ExtremeUnreal, Settings.ExtremeUnreal)
                .AddConfigCheckbox(Strings.Common.Labels.Criterion, Settings.CriterionDuties)
                .Draw();
            
            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }
        private const int SpiritBondStatusID = 49;
        private readonly Item SpiritBond;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            SpiritBond = Service.DataManager.GetExcelSheet<Item>()!.GetRow(27960)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.DisableInCombat.Value && Service.Condition[ConditionFlag.InCombat]) return null;

            switch (Service.DutyLists.GetDutyType(Service.ClientState.TerritoryType))
            {
                case DutyType.Savage when !Settings.SavageDuties.Value:
                case DutyType.Ultimate when !Settings.UltimateDuties.Value:
                case DutyType.ExtremeUnreal when !Settings.ExtremeUnreal.Value:
                case DutyType.Criterion when !Settings.CriterionDuties.Value:
                    
                case DutyType.None when Settings.SavageDuties.Value:
                case DutyType.None when Settings.UltimateDuties.Value:
                case DutyType.None when Settings.ExtremeUnreal.Value:
                case DutyType.None when Settings.CriterionDuties.Value:
                    return null;
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == SpiritBondStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.SpiritBondEarlyWarningTime.Value)
                return new WarningState
                {
                    MessageLong = Strings.Modules.SpiritBond.WarningText,
                    MessageShort = Strings.Modules.SpiritBond.WarningText,
                    IconID = SpiritBond.Icon,
                    IconLabel = Strings.Modules.SpiritBond.Label,
                    Priority = Settings.Priority.Value
                };

            return null;
        }
    }
}