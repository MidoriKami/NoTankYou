using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class SummonerModule : IModule
    {
        public List<ClassJob> ClassJobs { get; }
        private static SummonerModuleSettings Settings => Service.Configuration.ModuleSettings.Summoner;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Summoner.WarningText;

        public SummonerModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Where(job => job.RowId is 27 or 26)
                .ToList();
        }

        public bool ShowWarning(PlayerCharacter character)
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
