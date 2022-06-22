using System.Collections.Generic;
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
    internal class SageModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static SageModuleSettings Settings => Service.Configuration.ModuleSettings.Sage;
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.Sage.WarningText;
        public string MessageShort => Strings.Modules.Sage.WarningTextShort;
        public string ModuleCommand => "sge";

        private const int KardiaStatusID = 2604;

        private readonly Action KardiaAction;

        public SageModule()
        {
            ClassJobs = new List<uint> { 40 };

            KardiaAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(24285)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (!character.HasStatus(KardiaStatusID))
            {
                return new WarningState
                {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = KardiaAction.Icon,
                    IconLabel = KardiaAction.Name.RawString,
                    Priority = GenericSettings.Priority,
                    Sender = ModuleType.Sage,
                };
            }

            return null;
        }
    }
}
