using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules {
    internal class SpiritbondModule : IModule {
        public List<uint> ClassJobs { get; }
        private static SpiritbondModuleSettings Settings => Service.Configuration.ModuleSettings.Spiritbond;
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.Spiritbond.WarningText;
        public string MessageShort => Strings.Modules.Spiritbond.WarningText;
        public string ModuleCommand => "spiritbond";

        private const int SpiritbondStatusID = 49;
        private readonly List<uint> SavageDuties;
        private readonly List<uint> UltimateDuties;
        private readonly List<uint> ExtremeUnrealDuties;
        private readonly Item Spiritbond;

        public SpiritbondModule() {
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

            Spiritbond = Service.DataManager.GetExcelSheet<Item>()!.GetRow(27960)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character) {
            if (Settings.DisableInCombat && Service.Condition[ConditionFlag.InCombat]) return null;
            if (Settings.SavageDuties || Settings.UltimateDuties || Settings.ExtremeUnreal) {
                var inSavage = SavageDuties.Contains(Service.ClientState.TerritoryType);
                var inUltimate = UltimateDuties.Contains(Service.ClientState.TerritoryType);
                var inExtremeUnreal = ExtremeUnrealDuties.Contains(Service.ClientState.TerritoryType);
                var savageCheck = Settings.SavageDuties && inSavage;
                var ultimateCheck = Settings.UltimateDuties && inUltimate;
                var extremeUnrealCheck = Settings.ExtremeUnreal && inExtremeUnreal;
                if (!savageCheck && !ultimateCheck && !extremeUnrealCheck) return null;
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == SpiritbondStatusID);
            if (statusEffect == null || statusEffect.RemainingTime < Settings.SpiritbondEarlyWarningTime) {
                return new WarningState {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = Spiritbond.Icon,
                    IconLabel = Strings.Modules.Spiritbond.Label,
                    Priority = GenericSettings.Priority,
                    Sender = ModuleType.Spiritbond,
                };
            }

            return null;
        }
    }
}