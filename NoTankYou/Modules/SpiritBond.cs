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
        public ISelectable Selectable { get; }
        public ModuleConfigurationComponent(IModule parentModule)
        {
            Selectable = new ConfigurationSelectable(parentModule, this);
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Food_EarlyWarningLabel, out var innerWidth)
                .AddInputInt(Strings.Labels_Seconds, Settings.SpiritBondEarlyWarningTime, 0, 3600, 0, 0, innerWidth / 4.0f)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Common_AdditionalOptions)
                .AddConfigCheckbox(Strings.Food_SuppressInCombat, Settings.DisableInCombat)
                .Draw();

            if (!Settings.EnableZoneFilter)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Food_ZoneFilters)
                    .AddString(Strings.Food_ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Food_EnableFilter, Settings.EnableZoneFilter)
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Food_ZoneFilters)
                    .AddString(Strings.Food_ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Food_EnableFilter, Settings.EnableZoneFilter)
                    .Indent(15)
                    .AddConfigCheckbox(Strings.Labels_Savage, Settings.SavageDuties)
                    .AddConfigCheckbox(Strings.Labels_Ultimate, Settings.UltimateDuties)
                    .AddConfigCheckbox(Strings.Labels_ExtremeUnreal, Settings.ExtremeUnreal)
                    .AddConfigCheckbox(Strings.Labels_Criterion, Settings.CriterionDuties)
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
            if (Settings.DisableInCombat && Condition.IsInCombat()) return null;

            if (Settings.EnableZoneFilter)
            {
                var allowedTypes = new List<DutyType>();
                
                if(Settings.SavageDuties) allowedTypes.Add(DutyType.Savage);
                if(Settings.UltimateDuties) allowedTypes.Add(DutyType.Ultimate);
                if(Settings.ExtremeUnreal) allowedTypes.Add(DutyType.ExtremeUnreal);
                if(Settings.CriterionDuties) allowedTypes.Add(DutyType.Savage);

                if (!DutyLists.Instance.IsType(Service.ClientState.TerritoryType, allowedTypes))
                {
                    return null;
                }
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == SpiritBondStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.SpiritBondEarlyWarningTime.Value)
                return new WarningState
                {
                    MessageLong = Strings.SpiritBond_WarningText,
                    MessageShort = Strings.SpiritBond_WarningText,
                    IconID = SpiritBond.Icon,
                    IconLabel = Strings.SpiritBond_Label,
                    Priority = Settings.Priority.Value,
                };

            return null;
        }
    }
}