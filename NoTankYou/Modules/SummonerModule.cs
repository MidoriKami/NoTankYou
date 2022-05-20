using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class SummonerModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static SummonerModuleSettings Settings => Service.Configuration.ModuleSettings.Summoner;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Summoner.WarningText;

        public SummonerModule()
        {
            ClassJobs = new List<uint> {27, 26};
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            var hasPet = HasPet(character);

            return !hasPet;
        }

        private bool HasPet(PlayerCharacter character)
        {
            var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == character.ObjectId);

            return ownedObjects.Any(obj => obj.ObjectKind == ObjectKind.BattleNpc && (obj as BattleNpc)?.SubKind == (byte) BattleNpcSubKind.Pet);
        }
    }
}
