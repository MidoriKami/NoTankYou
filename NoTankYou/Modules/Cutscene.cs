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

namespace NoTankYou.Modules;

public class CutsceneConfiguration : GenericSettings
{
    public Setting<bool> CheckAlliance = new(true);
}

internal class Cutscene : IModule
{
    public ModuleName Name => ModuleName.Cutscene;
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static CutsceneConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Cutscene;
    public GenericSettings GenericSettings => Settings;

    public Cutscene()
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
            InfoBox.Instance
                .AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10)
                .Draw();
            
            InfoBox.Instance
                .AddTitle(Strings.Common.Labels.AdditionalOptions)
                .AddConfigCheckbox(Strings.Modules.Cutscene.CheckAlliance, Settings.CheckAlliance)
                .Draw();
            
            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private readonly OnlineStatus CutsceneStatus;
        
        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            
            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            CutsceneStatus = Service.DataManager.GetExcelSheet<OnlineStatus>()!
                .GetRow(15)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.OnlineStatus.Id == CutsceneStatus.RowId)
            {
                return new WarningState
                {
                    MessageLong = Strings.Modules.Cutscene.WarningText,
                    MessageShort = Strings.Modules.Cutscene.WarningTextShort,
                    IconID = CutsceneStatus.Icon,
                    IconLabel = CutsceneStatus.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            if (Settings.CheckAlliance.Value && Service.ClientState.LocalPlayer?.ObjectId == character.ObjectId)
            {
                var allianceMembers = GetAllianceMembers();
                
                if (allianceMembers.Any(member => member.OnlineStatus.Id == CutsceneStatus.RowId ))
                {
                    return new WarningState
                    {
                        MessageLong = Strings.Modules.Cutscene.WarningText,
                        MessageShort = Strings.Modules.Cutscene.WarningTextShort,
                        IconID = CutsceneStatus.Icon,
                        IconLabel = CutsceneStatus.Name.RawString,
                        Priority = Settings.Priority.Value,
                    };
                }
            }

            return null;
        }
        
        private IEnumerable<PlayerCharacter> GetAllianceMembers()
        {
            var players = new List<PlayerCharacter>();

            for (var i = 0; i < 16; ++i)
            {
                var player = HudAgent.GetAllianceMember(i);
                if(player == null) continue;

                players.Add(player);
            }

            return players;
        }
    }
}