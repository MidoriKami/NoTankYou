using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Extensions;
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

            var hasPet = character.HasPet();
            var hasDissipation = character.HasStatus(DissipationStatusID);

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
    }
}
