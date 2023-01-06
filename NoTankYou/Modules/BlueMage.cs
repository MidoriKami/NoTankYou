using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;
using Condition = KamiLib.Utilities.Condition;

namespace NoTankYou.Modules;

public class BlueMageConfiguration : GenericSettings
{
    public Setting<bool> Mimicry = new(false);
    public Setting<bool> TankStance = new(false);
}

public class BlueMage : IModule
{
    public ModuleName Name => ModuleName.BlueMage;

    public IConfigurationComponent ConfigurationComponent { get; }
    public ILogicComponent LogicComponent { get; }

    private static BlueMageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.BlueMage;
    public GenericSettings GenericSettings => Settings;

    public BlueMage()
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
            InfoBox.Instance
                .AddTitle(Strings.Tabs_Settings)
                .AddConfigCheckbox(Strings.Labels_Enabled, Settings.Enabled)
                .AddInputInt(Strings.Labels_Priority, Settings.Priority, 0, 10)
                .Draw();
            
            InfoBox.Instance
                .AddTitle(Strings.Labels_Warnings)
                .AddConfigCheckbox(Strings.BlueMage_MimicryLabel, Settings.Mimicry)
                .AddConfigCheckbox(Strings.BlueMage_MightyGuardLabel, Settings.TankStance)
                .Draw();

            InfoBox.Instance.DrawOverlaySettings(Settings);
            
            InfoBox.Instance.DrawOptions(Settings);
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }
        
        private readonly List<uint> MimicryStatusEffects;
        private const uint MightyGuardStatusEffect = 1719;
        private const uint AethericMimicryTank = 2124;

        private readonly Action MimicryAction;
        private readonly Action MightyGuardAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = new List<uint> { 36 };

            MimicryStatusEffects = new List<uint>{ 2124, 2125, 2126 };
            
            MimicryAction = LuminaCache<Action>.Instance.GetRow(18322)!;
            MightyGuardAction = LuminaCache<Action>.Instance.GetRow(11417)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.Mimicry && !Condition.IsBoundByDuty() && !character.HasStatus(MimicryStatusEffects))
            {
                return MimicryWarning();
            }

            if (Settings.TankStance)
            {
                if (Service.PartyList.Length == 0 && character.HasStatus(AethericMimicryTank) && !character.HasStatus(MightyGuardStatusEffect))
                {
                    return TankWarning();
                }
                else
                {
                    var tankMages = Service.PartyList
                        .WithJob(ClassJobs)
                        .Alive()
                        .WithStatus(AethericMimicryTank)
                        .ToList();

                    if (tankMages.Any() && !tankMages.WithStatus(MightyGuardStatusEffect).Any())
                    { 
                        return TankWarning();
                    }
                }
            }
            
            return null;
        }

        private WarningState MimicryWarning()
        {
            return new WarningState
            {
                MessageShort = Strings.BlueMage_MimicryLabel,
                MessageLong = Strings.BlueMage_Mimicry,
                IconID = MimicryAction.Icon,
                IconLabel = MimicryAction.Name.ToString(),
                Priority = Settings.Priority.Value,
            };
        }
        
        private WarningState TankWarning()
        {
            return new WarningState
            {
                MessageShort = Strings.BlueMage_MightyGuardLabel,
                MessageLong = Strings.BlueMage_MightyGuard,
                IconID = MightyGuardAction.Icon,
                IconLabel = MightyGuardAction.Name.ToString(),
                Priority = Settings.Priority.Value,
            };
        }
    }
}