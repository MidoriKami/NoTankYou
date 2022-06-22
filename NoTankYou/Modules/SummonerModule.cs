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

            if(!character.HasPet())
            {
                return new WarningState
                {
                    MessageLong = MessageLong,
                    MessageShort = MessageShort,
                    IconID = SummonCarbuncle.Icon,
                    IconLabel = SummonCarbuncle.Name.RawString,
                    Priority = GenericSettings.Priority,
                    Sender = ModuleType.Summoner,
                };
            }

            return null;
        }
    }
}
