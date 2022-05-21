using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class BlueMageModule : IModule
    {
        public List<uint> ClassJobs { get; }
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.BlueMage.GenericWarning;
        private static BlueMageModuleSettings Settings => Service.Configuration.ModuleSettings.BlueMage;

        private readonly List<uint> MimicryStatusEffects;
        private readonly uint MightyGuardStatusEffect = 1719;
        private readonly uint BasicInstinct = 2498;

        private string ActualWarning = Strings.Modules.BlueMage.GenericWarning;

        public BlueMageModule()
        {
            ClassJobs = new List<uint> { 36 };

            MimicryStatusEffects = new List<uint>{ 2124, 2125, 2126 };
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.Mimicry && !Service.EventManager.DutyStarted && !character.StatusList.Any(status => MimicryStatusEffects.Contains(status.StatusId)))
            {
                ActualWarning = Strings.Modules.BlueMage.Mimicry;
                return true;
            }

            if (Settings.TankStance && !character.StatusList.Any(status => status.StatusId == MightyGuardStatusEffect))
            {
                ActualWarning = Strings.Modules.BlueMage.MightyGuard;
                return true;
            }

            if (Settings.BasicInstinct && !character.StatusList.Any(status => status.StatusId == BasicInstinct))
            {
                ActualWarning = Strings.Modules.BlueMage.BasicInstinct;
                return true;
            }

            return false;
        }

        WarningState IModule.GetWarningState()
        {
            return new WarningState
            {
                Message = ActualWarning,
                Priority = GenericSettings.Priority,
            };
        }
    }
}
