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

internal class Food : IModule
{
    public ModuleName Name => ModuleName.Food;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static FoodConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Food;
    public GenericSettings GenericSettings => Settings;

    public Food()
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
                .AddTitle(Strings.Modules.Food.EarlyWarningLabel)
                .AddInputInt(Strings.Common.Labels.Seconds, Settings.FoodEarlyWarningTime, 0, 3600, 0, 0, 100.0f)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Modules.Food.AdditionalOptionsLabel)
                .AddConfigCheckbox(Strings.Modules.Food.SuppressInCombat, Settings.DisableInCombat)
                .Draw();

            if (!Settings.EnableZoneFilter.Value)
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
                    .Indent(-15)
                    .Draw();
            }

            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private const int WellFedStatusID = 48;

        private readonly Item Food;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();
            
            Food = Service.DataManager.GetExcelSheet<Item>()!.GetRow(30482)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.DisableInCombat.Value && Service.Condition[ConditionFlag.InCombat]) return null;

            if (Settings.EnableZoneFilter.Value)
            {
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