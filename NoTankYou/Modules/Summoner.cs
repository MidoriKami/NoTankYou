using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.Enums;
using NoTankYou.Configuration.ModuleSettings;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

internal class Summoner : IModule
{
    public ModuleName Name => ModuleName.Summoner;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static SummonerConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Summoner;
    public GenericSettings GenericSettings => Settings;

    public Summoner()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    internal class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox GenericSettings = new();
        private readonly InfoBox OverlaySettings = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            GenericSettings
                .AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Configuration.SoloMode, Settings.SoloMode, Strings.Configuration.SoloModeHelp)
                .AddConfigCheckbox(Strings.Configuration.DutiesOnly, Settings.DutiesOnly, Strings.Configuration.DutiesOnlyHelp)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10)
                .Draw();

            OverlaySettings
                .AddTitle(Strings.Common.Labels.DisplayOptions)
                .AddConfigCheckbox(Strings.TabItems.PartyOverlay.Label, Settings.PartyFrameOverlay)
                .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Label, Settings.BannerOverlay)
                .Draw();

        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private readonly Action SummonCarbuncle;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
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
                    MessageLong = Strings.Modules.Summoner.WarningText,
                    MessageShort = Strings.Modules.Summoner.WarningTextShort,
                    IconID = SummonCarbuncle.Icon,
                    IconLabel = SummonCarbuncle.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}