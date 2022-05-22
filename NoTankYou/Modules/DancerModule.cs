using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class DancerModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static DancerModuleSettings Settings => Service.Configuration.ModuleSettings.Dancer;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Dancer.WarningText;
        public string ModuleCommand => "dnc";

        private const int ClosedPositionStatusId = 1823;

        public DancerModule()
        {
            ClassJobs = new List<uint> { 38 };
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 60) return false;
            if (Service.PartyList.Length < 2) return false;

            return !character.StatusList.Any(s => s.StatusId is ClosedPositionStatusId);
        }
    }
}
