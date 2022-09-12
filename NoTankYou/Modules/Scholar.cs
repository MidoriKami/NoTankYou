using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class ScholarConfiguration :GenericSettings
{
}

internal class Scholar : IModule
{
    public ModuleName Name => ModuleName.Scholar;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static ScholarConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Scholar;
    public GenericSettings GenericSettings => Settings;

    public Scholar()
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

        private const int DissipationStatusID = 791;

        private readonly HashSet<uint> CharacterWaitList = new();

        private bool LastDissipationStatus;

        private readonly Action SeleneAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = new List<uint> { 28 };

            SeleneAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(17216)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 4) return null;

            var hasPet = character.HasPet();
            var hasDissipation = character.HasStatus(DissipationStatusID);

            // If we had dissipation last frame, but not now, wait a half second
            if (LastDissipationStatus && !hasDissipation)
            {
                CharacterWaitList.Add(character.ObjectId);
                Task.Delay(500).ContinueWith(_ =>
                {
                    CharacterWaitList.Remove(character.ObjectId);
                });
            }

            LastDissipationStatus = hasDissipation;

            if (CharacterWaitList.Contains(character.ObjectId)) return null;
            if (!hasPet && !hasDissipation)
            {
                return new WarningState
                {
                    MessageLong = Strings.Modules.Scholar.WarningText,
                    MessageShort = Strings.Modules.Scholar.WarningTextShort,
                    IconID = SeleneAction.Icon,
                    IconLabel = SeleneAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}
