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

public class SageConfiguration : GenericSettings
{
}

public class Sage : IModule
{
    public ModuleName Name => ModuleName.Sage;
    public string Command => "sge";
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    private static SageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Sage;
    public GenericSettings GenericSettings => Settings;

    public Sage()
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

        private const int KardiaStatusID = 2604;

        private readonly Action KardiaAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = new List<uint> { 40 };

            KardiaAction = LuminaCache<Action>.Instance.GetRow(24285)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 4) return null;

            if (!character.HasStatus(KardiaStatusID))
            {
                return new WarningState
                {
                    MessageLong = Strings.Sage_WarningText,
                    MessageShort = Strings.Sage_WarningTextShort,
                    IconID = KardiaAction.Icon,
                    IconLabel = KardiaAction.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}