using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    internal class ScholarModule : IModule
    {
        public List<ClassJob> ClassJobs { get; }
        private static ScholarModuleSettings Settings => Service.Configuration.ModuleSettings.Scholar;
        public GenericSettings GenericSettings => Settings;

        private const int DissipationStatusID = 791;

        private readonly HashSet<uint> CharacterWaitList = new();

        private bool LastDissipationStatus;
        public ScholarModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Where(job => job.RowId is 28)
                .ToList();

            Settings.WarningText = Strings.Modules.Scholar.WarningText;
        }

        public bool ShowWarning(PlayerCharacter character)
        {
            var hasPet = HasPet(character);
            var hasDissipation = HasDissipation(character);

            // If we had dissipation last frame, but not now, wait a half second
            if (LastDissipationStatus && !hasDissipation)
            {
                CharacterWaitList.Add(character.ObjectId);
                Task.Delay(500).ContinueWith(t =>
                {
                    CharacterWaitList.Remove(character.ObjectId);
                });
            }

            LastDissipationStatus = hasDissipation;

            if (CharacterWaitList.Contains(character.ObjectId)) return false;

            return !hasPet && !hasDissipation;
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
