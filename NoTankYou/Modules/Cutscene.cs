using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class CutsceneConfiguration : GenericSettings
{
}

public class Cutscene : IModule
{
    public ModuleName Name => ModuleName.Cutscene;
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static CutsceneConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Cutscene;
    public GenericSettings GenericSettings => Settings;

    public Cutscene()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
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
            
            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private readonly OnlineStatus CutsceneStatus;

        private static readonly Dictionary<uint, Stopwatch> TimeSinceInCutscene = new();
        
        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            
            ClassJobs = LuminaCache<ClassJob>.Instance
                .Select(r => r.RowId)
                .ToList();

            CutsceneStatus = LuminaCache<OnlineStatus>.Instance.GetRow(15)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            TimeSinceInCutscene.TryAdd(character.ObjectId, Stopwatch.StartNew());
            var stopwatch = TimeSinceInCutscene[character.ObjectId];

            if (stopwatch.Elapsed >= TimeSpan.FromSeconds(1) && character.HasOnlineStatus(CutsceneStatus.RowId))
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
            else if (!character.HasOnlineStatus(CutsceneStatus.RowId))
            {
                stopwatch.Restart();
            }

            return null;
        }
    }
}