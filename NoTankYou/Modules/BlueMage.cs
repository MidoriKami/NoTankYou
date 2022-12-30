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
using NoTankYou.Configuration;
using NoTankYou.DataModels;
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

    internal static BlueMageConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.BlueMage;
    public GenericSettings GenericSettings => Settings;

    public BlueMage()
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
            InfoBox.Instance
                .AddTitle(Strings.Common.Tabs.Settings)
                .AddConfigCheckbox(Strings.Common.Labels.Enabled, Settings.Enabled)
                .AddInputInt(Strings.Common.Labels.Priority, Settings.Priority, 0, 10)
                .Draw();
            
            InfoBox.Instance
                .AddTitle(Strings.Common.Labels.Warnings)
                .AddConfigCheckbox(Strings.Modules.BlueMage.MimicryLabel, Settings.Mimicry)
                .AddConfigCheckbox(Strings.Modules.BlueMage.MightyGuardLabel, Settings.TankStance)
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
            if (Settings.Mimicry.Value && !Condition.IsBoundByDuty() && !character.HasStatus(MimicryStatusEffects))
            {
                return MimicryWarning();
            }

            if (Settings.TankStance.Value)
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
                MessageShort = Strings.Modules.BlueMage.MimicryLabel,
                MessageLong = Strings.Modules.BlueMage.Mimicry,
                IconID = MimicryAction.Icon,
                IconLabel = MimicryAction.Name.ToString(),
                Priority = Settings.Priority.Value,
            };
        }
        
        private WarningState TankWarning()
        {
            return new WarningState
            {
                MessageShort = Strings.Modules.BlueMage.MightyGuardLabel,
                MessageLong = Strings.Modules.BlueMage.MightyGuard,
                IconID = MightyGuardAction.Icon,
                IconLabel = MightyGuardAction.Name.ToString(),
                Priority = Settings.Priority.Value,
            };
        }
    }
}