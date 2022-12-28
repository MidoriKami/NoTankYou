using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Extensions;
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
        
        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.DrawGenericSettings(Settings);
            
            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
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