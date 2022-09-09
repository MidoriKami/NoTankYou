using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.Enums;
using NoTankYou.Configuration.ModuleSettings;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;

namespace NoTankYou.Modules;

internal class SpiritBond : IModule {
    public ModuleName Name => ModuleName.SpiritBond;
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    internal static SpiritBondConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.SpiritBond;
    public GenericSettings GenericSettings => Settings;

    public SpiritBond() {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    internal class ModuleConfigurationComponent : IConfigurationComponent {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);
        private readonly InfoBox GenericSettings = new();
        private readonly InfoBox OverlaySettings = new();
        private readonly InfoBox ExtraOptions = new();
        private readonly InfoBox ZoneFilters = new();
        private readonly InfoBox AdditionalOptions = new();

        public ModuleConfigurationComponent(IModule parentModule) {
            ParentModule = parentModule;
        }

        public void Draw() {
            GenericSettings.AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Configuration.SoloMode, Settings.SoloMode,
                    Strings.Configuration.SoloModeHelp)
                .AddConfigCheckbox(Strings.Configuration.DutiesOnly, Settings.DutiesOnly,
                    Strings.Configuration.DutiesOnlyHelp)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10).Draw();
            ExtraOptions.AddTitle(Strings.Modules.SpiritBond.EarlyWarningLabel)
                .AddInputInt(Strings.Common.Labels.Seconds, Settings.SpiritBondEarlyWarningTime, 0, 3600, 100.0f)
                .Draw();
            AdditionalOptions.AddTitle(Strings.Modules.SpiritBond.AdditionalOptionsLabel)
                .AddConfigCheckbox(Strings.Modules.SpiritBond.SuppressInCombat, Settings.DisableInCombat).Draw();
            ZoneFilters.AddTitle(Strings.Modules.SpiritBond.ZoneFilters)
                .AddString(Strings.Modules.SpiritBond.ZoneFiltersDescription)
                .AddConfigCheckbox(Strings.Common.Labels.Savage, Settings.SavageDuties)
                .AddConfigCheckbox(Strings.Common.Labels.Ultimate, Settings.UltimateDuties)
                .AddConfigCheckbox(Strings.Common.Labels.ExtremeUnreal, Settings.ExtremeUnreal).Draw();
            OverlaySettings.AddTitle(Strings.Common.Labels.DisplayOptions)
                .AddConfigCheckbox(Strings.TabItems.PartyOverlay.Label, Settings.PartyFrameOverlay)
                .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Label, Settings.BannerOverlay).Draw();
        }
    }

    internal class ModuleLogicComponent : ILogicComponent {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }
        private const int SpiritBondStatusID = 49;
        private readonly List<uint> SavageDuties;
        private readonly List<uint> UltimateDuties;
        private readonly List<uint> ExtremeUnrealDuties;
        private readonly Item SpiritBond;

        public ModuleLogicComponent(IModule parentModule) {
            ParentModule = parentModule;
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!.Select(r => r.RowId).ToList();

            // ContentType.Row 5 == Raids
            SavageDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
                .Where(t => t.ContentType.Row == 5).Where(t => t.Name.RawString.Contains("Savage"))
                .Select(r => r.TerritoryType.Row).ToList();

            // ContentType.Row 28 == Ultimate Raids
            UltimateDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
                .Where(t => t.ContentType.Row == 28).Select(t => t.TerritoryType.Row).ToList();

            // ContentType.Row 4 == Trials
            ExtremeUnrealDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
                .Where(t => t.ContentType.Row == 4)
                .Where(t => t.Name.RawString.Contains("Extreme") || t.Name.RawString.Contains("Unreal"))
                .Select(t => t.TerritoryType.Row).ToList();
            SpiritBond = Service.DataManager.GetExcelSheet<Item>()!.GetRow(27960)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character) {
            if (Settings.DisableInCombat.Value && Service.Condition[ConditionFlag.InCombat]) return null;
            if (Settings.SavageDuties.Value || Settings.UltimateDuties.Value || Settings.ExtremeUnreal.Value) {
                var inSavage = SavageDuties.Contains(Service.ClientState.TerritoryType);
                var inUltimate = UltimateDuties.Contains(Service.ClientState.TerritoryType);
                var inExtremeUnreal = ExtremeUnrealDuties.Contains(Service.ClientState.TerritoryType);
                var savageCheck = Settings.SavageDuties.Value && inSavage;
                var ultimateCheck = Settings.UltimateDuties.Value && inUltimate;
                var extremeUnrealCheck = Settings.ExtremeUnreal.Value && inExtremeUnreal;
                if (!savageCheck && !ultimateCheck && !extremeUnrealCheck) return null;
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == SpiritBondStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.SpiritBondEarlyWarningTime.Value) {
                return new WarningState {
                    MessageLong = Strings.Modules.SpiritBond.WarningText,
                    MessageShort = Strings.Modules.SpiritBond.WarningText,
                    IconID = SpiritBond.Icon,
                    IconLabel = Strings.Modules.SpiritBond.Label,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}