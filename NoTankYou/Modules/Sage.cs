using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class SageConfiguration : GenericSettings
{
}

internal class Sage : IModule
{
    public ModuleName Name => ModuleName.Sage;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    internal static SageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Sage;
    public GenericSettings GenericSettings => Settings;

    public Sage()
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
            InfoBox.Instance.DrawGenericSettings(Settings);
            
            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }

        private const int KardiaStatusID = 2604;

        private readonly Action KardiaAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = new List<uint> { 40 };

            KardiaAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(24285)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 4) return null;

            if (!character.HasStatus(KardiaStatusID))
            {
                return new WarningState
                {
                    MessageLong = Strings.Modules.Sage.WarningText,
                    MessageShort = Strings.Modules.Sage.WarningTextShort,
                    IconID = KardiaAction.Icon,
                    IconLabel = KardiaAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}