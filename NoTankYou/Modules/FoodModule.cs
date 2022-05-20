using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class FoodModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static FoodModuleSettings Settings => Service.Configuration.ModuleSettings.Food;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Food.WarningText;

        private const int WellFedStatusID = 48;
        private readonly List<uint> SavageDuties;
        private readonly List<uint> UltimateDuties;

        public FoodModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            // ContentType.Row 5 == Raids
            SavageDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
                .Where(t => t.ContentType.Row == 5)
                .Where(t => t.Name.RawString.Contains("Savage"))
                .Select(r => r.TerritoryType.Row)
                .ToList();

            // ContentType.Row 28 == Ultimate Raids
            UltimateDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
                .Where(t => t.ContentType.Row == 28)
                .Select(t => t.TerritoryType.Row)
                .ToList();
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.SavageDuties || Settings.UltimateDuties)
            {
                var inSavage = SavageDuties.Contains(Service.ClientState.TerritoryType);
                var inUltimate = UltimateDuties.Contains(Service.ClientState.TerritoryType);

                var savageCheck = Settings.SavageDuties && inSavage;
                var ultimateCheck = Settings.UltimateDuties && inUltimate;

                if (!savageCheck && !ultimateCheck) return false;
            }

            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == WellFedStatusID);

            if (statusEffect == null) return true;

            return statusEffect.RemainingTime < Settings.FoodEarlyWarningTime;
        }
    }
}
