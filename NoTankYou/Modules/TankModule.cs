using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using Action = Lumina.Excel.GeneratedSheets.Action;
using Status = Lumina.Excel.GeneratedSheets.Status;

namespace NoTankYou.Modules
{
    internal class TankModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static TankModuleSettings Settings => Service.Configuration.ModuleSettings.Tank;
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.Tank.WarningText;

        private readonly List<uint> TankStances;

        private readonly HashSet<uint> AllianceRaidTerritories;

        public TankModule()
        {
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Where(job => job.Role is 1)
                .Select(r => r.RowId)
                .ToList();

            TankStances = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role == 1)
                .Select(r => r.StatusGainSelf.Value!)
                .Where(r => r.IsPermanent)
                .Select(s => s.RowId)
                .ToList();

            // Territory Intended Use 8 = Alliance Raid
            AllianceRaidTerritories = Service.DataManager.GetExcelSheet<TerritoryType>()
                !.Where(r => r.TerritoryIntendedUse is 8)
                .Select(r => r.RowId)
                .ToHashSet();
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.DisableInAllianceRaid && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType)) return false;

            if (Service.PartyList.Length == 0)
            {
                return !character.StatusList.Any(status => TankStances.Contains(status.StatusId));
            }

            return !Service.PartyList.Where(partyMember => partyMember.CurrentHP > 0 && ClassJobs.Contains(partyMember.ClassJob.Id))
                .Any(tanks => tanks.Statuses.Any(status => TankStances.Contains(status.StatusId)));
        }
    }
}
