using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;
using Condition = KamiLib.Utilities.Condition;

namespace NoTankYou.Modules;

public class SpiritBondConfiguration : GenericSettings 
{
    public Setting<int> SpiritBondEarlyWarningTime = new(600);
    public Setting<bool> SavageDuties = new(false);
    public Setting<bool> UltimateDuties = new(false);
    public Setting<bool> ExtremeUnreal = new(false);
    public Setting<bool> DisableInCombat = new(false);
    public Setting<bool> CriterionDuties = new(false);
    public Setting<bool> EnableZoneFilter = new(false);
}

public class Spiritbond : IModule
{
    public ModuleName Name => ModuleName.Spiritbond;
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    private static SpiritBondConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.SpiritBond;
    public GenericSettings GenericSettings => Settings;

    public Spiritbond()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
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
                .AddTitle(Strings.Modules.SpiritBond.EarlyWarningLabel, out var innerWidth)
                .AddInputInt(Strings.Common.Labels.Seconds, Settings.SpiritBondEarlyWarningTime, 0, 3600, 0, 0, innerWidth / 4.0f)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.SpiritBond.AdditionalOptionsLabel)
                .AddConfigCheckbox(Strings.Modules.SpiritBond.SuppressInCombat, Settings.DisableInCombat)
                .Draw();

            if (!Settings.EnableZoneFilter.Value)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Modules.SpiritBond.ZoneFilters)
                    .AddString(Strings.Modules.SpiritBond.ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Modules.SpiritBond.EnableFilter, Settings.EnableZoneFilter)
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Modules.SpiritBond.ZoneFilters)
                    .AddString(Strings.Modules.SpiritBond.ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Modules.SpiritBond.EnableFilter, Settings.EnableZoneFilter)
                    .Indent(15)
                    .AddConfigCheckbox(Strings.Common.Labels.Savage, Settings.SavageDuties)
                    .AddConfigCheckbox(Strings.Common.Labels.Ultimate, Settings.UltimateDuties)
                    .AddConfigCheckbox(Strings.Common.Labels.ExtremeUnreal, Settings.ExtremeUnreal)
                    .AddConfigCheckbox(Strings.Common.Labels.Criterion, Settings.CriterionDuties)
                    .UnIndent(15)
                    .Draw();
            }
            
            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }
        private const int SpiritBondStatusID = 49;
        private readonly Item SpiritBond;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = LuminaCache<ClassJob>.Instance
                .Select(r => r.RowId)
                .ToList();

            SpiritBond = LuminaCache<Item>.Instance.GetRow(27960)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.DisableInCombat.Value && Condition.IsInCombat()) return null;

            if (Settings.EnableZoneFilter.Value)
            {
                var allowedTypes = new List<DutyType>();
                
                if(Settings.SavageDuties.Value) allowedTypes.Add(DutyType.Savage);
                if(Settings.UltimateDuties.Value) allowedTypes.Add(DutyType.Ultimate);
                if(Settings.ExtremeUnreal.Value) allowedTypes.Add(DutyType.ExtremeUnreal);
                if(Settings.CriterionDuties.Value) allowedTypes.Add(DutyType.Savage);

                if (!DutyLists.Instance.IsType(Service.ClientState.TerritoryType, allowedTypes))
                {
                    return null;
                }
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == SpiritBondStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.SpiritBondEarlyWarningTime.Value)
                return new WarningState
                {
                    MessageLong = Strings.Modules.SpiritBond.WarningText,
                    MessageShort = Strings.Modules.SpiritBond.WarningText,
                    IconID = SpiritBond.Icon,
                    IconLabel = Strings.Modules.SpiritBond.Label,
                    Priority = Settings.Priority.Value,
                };

            return null;
        }
    }
}