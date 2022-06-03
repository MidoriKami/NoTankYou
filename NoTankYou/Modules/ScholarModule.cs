using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class ScholarModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static ScholarModuleSettings Settings => Service.Configuration.ModuleSettings.Scholar;
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.Scholar.WarningText;
        public string MessageShort => Strings.Modules.Scholar.WarningTextShort;
        public string ModuleCommand => "sch";

        private const int DissipationStatusID = 791;

        private readonly HashSet<uint> CharacterWaitList = new();

        private bool LastDissipationStatus;

        private readonly Action SeleneAction;

        public ScholarModule()
        {
            ClassJobs = new List<uint> { 28 };

            SeleneAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(17216)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 4) return null;

            var hasPet = HasPet(character);
            var hasDissipation = HasDissipation(character);

            // If we had dissipation last frame, but not now, wait a half second
            if (LastDissipationStatus && !hasDissipation)
            {
                CharacterWaitList.Add(character.ObjectId);
                Task.Delay(500).ContinueWith(_ =>
                {
                    CharacterWaitList.Remove(character.ObjectId);
                });
            }

            LastDissipationStatus = hasDissipation;

            if (CharacterWaitList.Contains(character.ObjectId)) return null;
            if (!hasPet && !hasDissipation)
            {
                return new WarningState
                {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = SeleneAction.Icon,
                    IconLabel = SeleneAction.Name.RawString,
                    Priority = GenericSettings.Priority,
                    Sender = ModuleType.Scholar,
                };
            }

            return null;
        }

        private bool HasDissipation(PlayerCharacter character)
        { 
            return character.StatusList.Any(s => s.StatusId == DissipationStatusID);
        }

        private bool HasPet(PlayerCharacter character)
        {
            var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == character.ObjectId);

            return ownedObjects.Any(obj => obj.ObjectKind == ObjectKind.BattleNpc && (obj as BattleNpc)?.SubKind == (byte) BattleNpcSubKind.Pet);
        }
    }
}
