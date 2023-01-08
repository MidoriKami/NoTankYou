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

public class SummonerConfiguration : GenericSettings
{
}

public class Summoner : IModule
{
    public ModuleName Name => ModuleName.Summoner;
    public string Command => "smn";
    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }
    private static SummonerConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Summoner;
    public GenericSettings GenericSettings => Settings;

    public Summoner()
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

        private readonly Action SummonCarbuncle;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            ClassJobs = new List<uint> {27, 26};

            SummonCarbuncle = LuminaCache<Action>.Instance.GetRow(25798)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (character.Level < 2) return null;

            if(!character.HasPet())
            {
                return new WarningState
                {
                    MessageLong = Strings.Summoner_WarningText,
                    MessageShort = Strings.Summoner_WarningTextShort,
                    IconID = SummonCarbuncle.Icon,
                    IconLabel = SummonCarbuncle.Name.RawString,
                    Priority = Settings.Priority.Value,
                };
            }

            return null;
        }
    }
}