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

public class FoodConfiguration : GenericSettings
{
    public Setting<int> FoodEarlyWarningTime = new(600);
    public Setting<bool> SavageDuties = new(false);
    public Setting<bool> UltimateDuties = new(false);
    public Setting<bool> ExtremeUnreal = new(false);
    public Setting<bool> DisableInCombat = new(true);
    public Setting<bool> CriterionDuties = new(false);
    public Setting<bool> EnableZoneFilter = new(false);
}

public class Food : IModule
{
    public ModuleName Name => ModuleName.Food;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static FoodConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Food;
    public GenericSettings GenericSettings => Settings;

    public Food()
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
                .AddTitle(Strings.Modules.Food.EarlyWarningLabel, out var innerWidth)
                .AddInputInt(Strings.Common.Labels.Seconds, Settings.FoodEarlyWarningTime, 0, 3600, 0, 0, innerWidth / 4.0f)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.Food.AdditionalOptionsLabel)
                .AddConfigCheckbox(Strings.Modules.Food.SuppressInCombat, Settings.DisableInCombat)
                .Draw();

            if (!Settings.EnableZoneFilter)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Modules.Food.ZoneFilters)
                    .AddString(Strings.Modules.Food.ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Modules.Food.EnableFilter, Settings.EnableZoneFilter)
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Modules.Food.ZoneFilters)
                    .AddString(Strings.Modules.Food.ZoneFiltersDescription)
                    .AddConfigCheckbox(Strings.Modules.Food.EnableFilter, Settings.EnableZoneFilter)
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

        private const int WellFedStatusID = 48;

        private readonly Item Food;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = LuminaCache<ClassJob>.Instance
                .Select(r => r.RowId)
                .ToList();
            
            Food = LuminaCache<Item>.Instance.GetRow(30482)!;
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
            
            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == WellFedStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.FoodEarlyWarningTime.Value)
            {
                return new WarningState
                {
                    MessageLong = Strings.Modules.Food.WarningText,
                    MessageShort = Strings.Modules.Food.WarningText,
                    IconID = Food.Icon,
                    IconLabel = Strings.Modules.Food.Label,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}