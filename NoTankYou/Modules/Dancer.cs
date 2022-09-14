using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class DancerConfiguration : GenericSettings
{
}

internal class Dancer : IModule
{
    public ModuleName Name => ModuleName.Dancer;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static DancerConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Dancer;
    public GenericSettings GenericSettings => Settings;

    public Dancer()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox GenericSettings = new();
        private readonly InfoBox OverlaySettings = new();
        private readonly InfoBox Options = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            GenericSettings
                .AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Configuration.SoloMode, Settings.SoloMode,
                    Strings.Configuration.SoloModeHelp)
                .AddConfigCheckbox(Strings.Configuration.DutiesOnly, Settings.DutiesOnly,
                    Strings.Configuration.DutiesOnlyHelp)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10)
                .Draw();

            OverlaySettings
                .AddTitle(Strings.Common.Labels.DisplayOptions)
                .AddConfigCheckbox(Strings.TabItems.PartyOverlay.Label, Settings.PartyFrameOverlay)
                .AddConfigCheckbox(Strings.TabItems.BannerOverlay.Label, Settings.BannerOverlay)
                .Draw();

            Options
                .AddTitle(Strings.Common.Labels.Options)
                .AddConfigCheckbox(Strings.Configuration.HideInSanctuary, Settings.DisableInSanctuary)
                .Draw();
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private const int ClosedPositionStatusId = 1823;

        private readonly Action ClosedPositionAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = new List<uint> { 38 };

            ClosedPositionAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(16006)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 60) return null;
            if (Service.PartyList.Length < 2) return null;

            if (!character.HasStatus(ClosedPositionStatusId))
            {
                return new WarningState
                {
                    MessageLong = Strings.Modules.Dancer.WarningText,
                    MessageShort = Strings.Modules.Dancer.WarningTextShort,
                    IconID = ClosedPositionAction.Icon,
                    IconLabel = ClosedPositionAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}