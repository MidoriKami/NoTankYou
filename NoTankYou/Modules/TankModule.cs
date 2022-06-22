using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Extensions;
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
            if (Settings.DisableInAllianceRaid && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType)) return null;

            if (Settings.CheckAllianceStances && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType))
            {
                return EvaluateAllianceStances(character);
            }

            if (Service.PartyList.Length == 0)
            {
                return character.HasStatus(TankStances) ? null : TankWarning(character);
            }
            else
            {
                return EvaluateParty(character);
            }
        }

        private WarningState? EvaluateParty(PlayerCharacter character)
        {
            var tanks = Service.PartyList
                .WithJob(ClassJobs)
                .Alive()
                .ToList();

            if (tanks.Any() && !tanks.WithStatus(TankStances).Any())
            {
                return TankWarning(character);
            }

            return null;
        }

        private WarningState? EvaluateAllianceStances(PlayerCharacter character)
        {
            var partyTanks = Service.PartyList
                .WithJob(ClassJobs)
                .Alive()
                .ToList();

            var allianceTanks = GetAllianceTanks();

            var partyMissingStance = !partyTanks.WithStatus(TankStances).Any();
            var allianceMissingStance = !allianceTanks.WithStatus(TankStances).Any();

            if (partyMissingStance && allianceMissingStance)
            {
                return TankWarning(character);
            }

            return null;
        }
        
        private unsafe uint GetAllianceMemberObjectID(int index)
        {
            var frameworkInstance = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
            var baseAddress = (byte*) frameworkInstance->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Hud);
            const int allianceDataOffset = 0x0E14;

            var objectId = *(uint*) (baseAddress + allianceDataOffset + index * 0x4);

            return objectId;
        }

        private IconInfo GetTankIcon(PlayerCharacter character)
        {
            // Convert certain jobs to base class,
            // because Paladin and Warrior don't actually have a tank stance
            uint classJob = character.ClassJob.Id switch
            {
                // Paladin => Gladiator
                19 => 1,

                // Warrior => Marauder
                21 => 3,
                _ => character.ClassJob.Id
            };

            var action = Service.DataManager.GetExcelSheet<Action>()!
                .Where(r => r.ClassJob.Row == classJob)
                .Where(r => TankStances.Contains(r.StatusGainSelf.Value!.RowId))
                .First();

            return new IconInfo
            {
                ID = action.Icon,
                Name = action.Name.RawString
            };
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

        private WarningState TankWarning(PlayerCharacter character)
        {
            var iconInfo = GetTankIcon(character);

            return new WarningState
            {
                MessageShort = MessageShort,
                MessageLong = MessageLong,
                IconID = iconInfo.ID,
                IconLabel = iconInfo.Name,
                Priority = GenericSettings.Priority,
                Sender = ModuleType.Tanks,
            };
        }
    }
}
