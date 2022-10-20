using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

internal record IconInfo(uint ID, string Name);

public class TankConfiguration : GenericSettings
{
    public Setting<bool> DisableInAllianceRaid = new(true);
    public Setting<bool> CheckAllianceStances = new(false);
}

internal class Tanks : IModule
{
    public ModuleName Name => ModuleName.Tanks;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static TankConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Tank;
    public GenericSettings GenericSettings => Settings;

    public Tanks()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    internal class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);
        
        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Common.Labels.AdditionalOptions)
                .AddConfigCheckbox(Strings.Modules.Tank.DisableInAllianceRaid, Settings.DisableInAllianceRaid)
                .AddConfigCheckbox(Strings.Modules.Tank.CheckAllianceStances, Settings.CheckAllianceStances)
                .Draw();
            
            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        
        public List<uint> ClassJobs { get; }

        private readonly List<uint> TankStances;
        private readonly HashSet<uint> AllianceRaidTerritories;
        private readonly Dictionary<uint, IconInfo> TankIcons = new();

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

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

            foreach (var job in ClassJobs)
            {
                var icon = GetTankIcon(job);

                TankIcons.Add(job, icon);
            }
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 10) return null;

            if (Settings.DisableInAllianceRaid.Value && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType)) return null;

            if (Settings.CheckAllianceStances.Value && AllianceRaidTerritories.Contains(Service.ClientState.TerritoryType))
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
        
        private IconInfo GetTankIcon(uint classjob)
        {
            // Convert certain jobs to base class,
            // because Paladin and Warrior don't actually have a tank stance
            uint translatedClassJob = classjob switch
            {
                // Paladin => Gladiator
                19 => 1,

                // Warrior => Marauder
                21 => 3,
                _ => classjob
            };

            var action = Service.DataManager.GetExcelSheet<Action>()!
                .Where(r => r.ClassJob.Row == translatedClassJob)
                .Where(r => TankStances.Contains(r.StatusGainSelf.Value!.RowId))
                .First();

            return new IconInfo(action.Icon, action.Name.RawString);
        }

        private IEnumerable<PlayerCharacter> GetAllianceTanks()
        {
            var players = new List<PlayerCharacter>();

            for (var i = 0; i < 16; ++i)
            {
                var player = HudAgent.GetAllianceMember(i);
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
            var iconInfo = TankIcons[character.ClassJob.Id];

            return new WarningState
            {
                MessageShort = Strings.Modules.Tank.WarningTextShort,
                MessageLong = Strings.Modules.Tank.WarningText,
                IconID = iconInfo.ID,
                IconLabel = iconInfo.Name,
                Priority = Settings.Priority.Value,
            };
        }
    }
}