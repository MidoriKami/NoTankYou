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
        public List<uint> ClassJobs { get; }
        private static SageModuleSettings Settings => Service.Configuration.ModuleSettings.Sage;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Sage.WarningText;

        private const int KardiaStatusID = 2604;

        public SageModule()
        {
            ClassJobs = new List<uint> { 40 };
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            return !character.StatusList.Any(p => p.StatusId == KardiaStatusID);
        }
    }
}
