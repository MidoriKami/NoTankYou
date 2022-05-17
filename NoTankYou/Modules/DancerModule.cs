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
    internal class DancerModule : IModule
    {
        public List<ClassJob> ClassJobs { get; }
        private static DancerModuleSettings Settings => Service.Configuration.ModuleSettings.Dancer;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Dancer.WarningText;

        private const int ClosedPositionStatusId = 1823;

        public DancerModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Where(job => job.RowId is 38)
                .ToList();
        }

        public bool ShowWarning(PlayerCharacter character)
        {
            if (character.Level < 60) return false;

            return !character.StatusList.Any(s => s.StatusId is ClosedPositionStatusId);
        }
    }
}
