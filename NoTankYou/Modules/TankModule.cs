using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.Modules
{
    internal class TankModule : IModule
    {
        public List<uint> ClassJobs { get; }
        private static TankModuleSettings Settings => Service.Configuration.ModuleSettings.Tank;
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.Tank.WarningText;
        public string MessageShort => Strings.Modules.Tank.WarningTextShort;
        public string ModuleCommand => "tank";

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

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            GetTankIcon(character);

            if (Settings.DisableInAllianceRaid && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType)) return null;

            if (Settings.CheckAllianceStances && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType))
            {
                var partyTanks = Service.PartyList.Where(m => ClassJobs.Contains(m.ClassJob.Id)).ToList();
                var allianceTanks = GetAllianceTanks();

                var partyMissingStance = !partyTanks.Any(tanks => tanks.Statuses.Any(status => TankStances.Contains(status.StatusId)));
                var allianceMissingStance = !allianceTanks.Any(tanks => tanks.StatusList.Any(status => TankStances.Contains(status.StatusId)));

                if (partyMissingStance && allianceMissingStance)
                {
                    return new WarningState
                    {
                        MessageShort = MessageShort,
                        MessageLong = MessageLong,
                        IconID = GetTankIcon(character).Item1,
                        IconLabel = GetTankIcon(character).Item2,
                        Priority = Settings.Priority,
                    };
                }
            }

            if (Service.PartyList.Length == 0)
            {
                if (!character.StatusList.Any(status => TankStances.Contains(status.StatusId)))
                {
                    return new WarningState
                    {
                        MessageShort = MessageShort,
                        MessageLong = MessageLong,
                        IconID = GetTankIcon(character).Item1,
                        IconLabel = GetTankIcon(character).Item2,
                        Priority = Settings.Priority,
                    };
                }
            }
            else
            {
                if (!Service.PartyList.Where(partyMember => partyMember.CurrentHP > 0 && ClassJobs.Contains(partyMember.ClassJob.Id))
                        .Any(tanks => tanks.Statuses.Any(status => TankStances.Contains(status.StatusId))))
                {
                    return new WarningState
                    {
                        MessageShort = MessageShort,
                        MessageLong = MessageLong,
                        IconID = GetTankIcon(character).Item1,
                        IconLabel = GetTankIcon(character).Item2,
                        Priority = Settings.Priority,
                    };
                }
            }

            return null;
        }

        private unsafe int GetAllianceMemberObjectID(int index)
        {
            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var baseAddress = (byte*) frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
            const int allianceDataOffset = 0x0E14;

            var objectId = *(int*) (baseAddress + allianceDataOffset + index * 0x4);

            return objectId;
        }

        private (uint, string) GetTankIcon(PlayerCharacter character)
        {
            var classJob = character.ClassJob.Id;

            if (classJob == 19)
                classJob = 1;

            if (classJob == 21)
                classJob = 3;

            var action = Service.DataManager.GetExcelSheet<Action>()!
                .Where(r => r.ClassJob.Row == classJob)
                .Where(r => TankStances.Contains(r.StatusGainSelf.Value!.RowId))
                .First();

            return (action.Icon, action.Name.RawString);
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
