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
    internal class SageModule : IModule
    {
        public List<ClassJob> ClassJobs { get; }
        private static SageModuleSettings Settings => Service.Configuration.ModuleSettings.Sage;
        public GenericSettings GenericSettings => Settings;

        private const int KardiaStatusID = 2604;

        public SageModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Where(job => job.RowId is 40)
                .ToList();

            Settings.WarningText = Strings.Modules.Sage.WarningText;
        }

        public bool ShowWarning(PlayerCharacter character)
        {
            return !character.StatusList.Any(p => p.StatusId == KardiaStatusID);
        }
    }
}
