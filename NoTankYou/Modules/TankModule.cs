using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;
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

            if (Settings.CheckAllianceStances && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType))
            {
                var partyTanks = Service.PartyList.Where(m => ClassJobs.Contains(m.ClassJob.Id)).ToList();
                var allianceTanks = GetAllianceTanks();

                var partyMissingStance = !partyTanks.Any(tanks => tanks.Statuses.Any(status => TankStances.Contains(status.StatusId)));
                var allianceMissingStance = !allianceTanks.Any(tanks => tanks.StatusList.Any(status => TankStances.Contains(status.StatusId)));

                return partyMissingStance && allianceMissingStance;
            }

            if (Service.PartyList.Length == 0)
            {
                return !character.StatusList.Any(status => TankStances.Contains(status.StatusId));
            }
            else
            {
                return !Service.PartyList.Where(partyMember => partyMember.CurrentHP > 0 && ClassJobs.Contains(partyMember.ClassJob.Id))
                    .Any(tanks => tanks.Statuses.Any(status => TankStances.Contains(status.StatusId)));
            }
        }

        private unsafe int GetAllianceMemberObjectID(int index)
        {
            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var baseAddress = (byte*) frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
            const int allianceDataOffset = 0x0E14;

            var objectId = *(int*) (baseAddress + allianceDataOffset + index * 0x4);

            return objectId;
        }

        private IEnumerable<PlayerCharacter> GetAllianceTanks()
        {
            var players = new List<PlayerCharacter>();

            for (var i = 0; i < 16; ++i)
            {
                var objectId = GetAllianceMemberObjectID(i);

                var player = PlayerLocator.GetPlayer(objectId);

                if(player == null) continue;

                if (ClassJobs.Contains(player.ClassJob.Id))
                {
                    players.Add(player);
                }
            }

            return players;
        }
    }
}
