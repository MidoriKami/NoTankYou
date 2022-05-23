using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
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
        public string MessageLong => Strings.Modules.Dancer.WarningText;
        public string MessageShort => Strings.Modules.Dancer.WarningTextShort;
        public string ModuleCommand => "dnc";

        private const int ClosedPositionStatusId = 1823;

        private readonly Action ClosedPositionAction;

        public DancerModule()
        {
            ClassJobs = new List<uint> { 38 };

            ClosedPositionAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(16006)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 60) return null;
            if (Service.PartyList.Length < 2) return null;

            if (!character.StatusList.Any(s => s.StatusId == ClosedPositionStatusId))
            {
                return new WarningState
                {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = ClosedPositionAction.Icon,
                    IconLabel = ClosedPositionAction.Name.RawString,
                    Priority = Settings.Priority
                };
            }

            return null;
        }
    }
}
