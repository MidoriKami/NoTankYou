using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Drawing;
using KamiLib.Extensions;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class ScholarConfiguration :GenericSettings
{
}

public class Scholar : IModule
{
    public ModuleName Name => ModuleName.Scholar;
    public string Command => "sch";
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    private static ScholarConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Scholar;
    public GenericSettings GenericSettings => Settings;

    public Scholar()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public ISelectable Selectable { get; }
        public ModuleConfigurationComponent(IModule parentModule)
        {
            Selectable = new ConfigurationSelectable(parentModule, this);
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(Settings);
            
            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private const int DissipationStatusID = 791;

        private readonly HashSet<uint> characterWaitList = new();

        private bool lastDissipationStatus;

        private readonly Action seleneAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = new List<uint> { 28 };

            seleneAction = LuminaCache<Action>.Instance.GetRow(17216)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 4) return null;

            var hasPet = character.HasPet();
            var hasDissipation = character.HasStatus(DissipationStatusID);

            // If we had dissipation last frame, but not now, wait a half second
            if (lastDissipationStatus && !hasDissipation)
            {
                characterWaitList.Add(character.ObjectId);
                Task.Delay(500).ContinueWith(_ =>
                {
                    characterWaitList.Remove(character.ObjectId);
                });
            }

            lastDissipationStatus = hasDissipation;

            if (characterWaitList.Contains(character.ObjectId)) return null;
            if (!hasPet && !hasDissipation)
            {
                return new WarningState
                {
                    MessageLong = Strings.Scholar_WarningText,
                    MessageShort = Strings.Scholar_WarningTextShort,
                    IconID = seleneAction.Icon,
                    IconLabel = seleneAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}
