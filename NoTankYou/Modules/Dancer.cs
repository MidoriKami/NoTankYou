using System.Collections.Generic;
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

public class DancerConfiguration : GenericSettings
{
}

public class Dancer : IModule
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

        private const int ClosedPositionStatusId = 1823;

        private readonly Action ClosedPositionAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = new List<uint> { 38 };

            ClosedPositionAction = LuminaCache<Action>.Instance.GetRow(16006)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 60) return null;
            if (Service.PartyList.Length < 2) return null;

            if (!character.HasStatus(ClosedPositionStatusId))
            {
                return new WarningState
                {
                    MessageLong = Strings.Dancer_WarningText,
                    MessageShort = Strings.Dancer_WarningTextShort,
                    IconID = ClosedPositionAction.Icon,
                    IconLabel = ClosedPositionAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}