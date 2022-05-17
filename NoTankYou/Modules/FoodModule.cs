using System.Collections.Generic;
using System.Linq;
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
        public List<ClassJob> ClassJobs { get; }
        private static FoodModuleSettings Settings => Service.Configuration.ModuleSettings.Food;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Food.WarningText;

        private const int WellFedStatusID = 48;
        private int FoodEarlyWarningTime = 600;

        public FoodModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!.ToList();
        }

        public bool ShowWarning(PlayerCharacter character)
        {
            var statusEffect = character.StatusList.FirstOrDefault(status => status.StatusId == WellFedStatusID);

            if (statusEffect == null) return true;

            return statusEffect.RemainingTime < FoodEarlyWarningTime;
        }
    }
}
