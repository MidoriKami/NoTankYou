using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
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
        public string MessageLong => Strings.Modules.Summoner.WarningText;
        public string MessageShort => Strings.Modules.Summoner.WarningTextShort;
        public string ModuleCommand => "smn";

        private readonly Action SummonCarbuncle;

        public SummonerModule()
        {
            ClassJobs = new List<uint> {27, 26};

            SummonCarbuncle = Service.DataManager.GetExcelSheet<Action>()!.GetRow(25798)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 2) return null;

            if(!HasPet(character))
            {
                return new WarningState
                {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = SummonCarbuncle.Icon,
                    IconLabel = SummonCarbuncle.Name.RawString,
                    Priority = Settings.Priority
                };
            }

            return null;
        }

        private bool HasPet(PlayerCharacter character)
        {
            var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == character.ObjectId);

            return ownedObjects.Any(obj => obj.ObjectKind == ObjectKind.BattleNpc && (obj as BattleNpc)?.SubKind == (byte) BattleNpcSubKind.Pet);
        }
    }
}
