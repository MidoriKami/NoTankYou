using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Components;
using NoTankYou.UserInterface.Components.InfoBox;
using NoTankYou.Utilities;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.Modules;

public class BlueMageConfiguration : GenericSettings
{
    public Setting<bool> Mimicry = new(false);
    public Setting<bool> TankStance = new(false);
    public Setting<bool> BasicInstinct = new(false);
}

internal class BlueMage : IModule
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
            InfoBox.DrawGenericSettings(Settings);
            
            InfoBox.Instance
                .AddTitle(Strings.Common.Labels.Warnings)
                .AddConfigCheckbox(Strings.Modules.BlueMage.MimicryLabel, Settings.Mimicry)
                .AddConfigCheckbox(Strings.Modules.BlueMage.MightyGuardLabel, Settings.TankStance)
                .AddConfigCheckbox(Strings.Modules.BlueMage.BasicInstinctLabel, Settings.BasicInstinct)
                .Draw();

            InfoBox.DrawOverlaySettings(Settings);
            
            InfoBox.DrawOptions(Settings);
        }
    }

    internal class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public List<uint> ClassJobs { get; }
        
        private readonly List<uint> MimicryStatusEffects;
        private readonly uint MightyGuardStatusEffect = 1719;
        private readonly uint BasicInstinct = 2498;
        private readonly uint AethericMimicryTank = 2124;

        private readonly Action MimicryAction;
        private readonly Action MightyGuardAction;
        private readonly Action BasicInstinctAction;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            ClassJobs = new List<uint> { 36 };

            MimicryStatusEffects = new List<uint>{ 2124, 2125, 2126 };

            MimicryAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(18322)!;
            MightyGuardAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(11417)!;
            BasicInstinctAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(23276)!;
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

            if (Settings.BasicInstinct.Value && !character.HasStatus(BasicInstinct))
            {
                if (Service.PartyList.Length == 0)
                {
                    return BasicInstinctWarning();
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

        private WarningState BasicInstinctWarning()
        {
            return new WarningState
            {
                MessageShort = Strings.Modules.BlueMage.BasicInstinctLabel,
                MessageLong = Strings.Modules.BlueMage.BasicInstinct,
                IconID = BasicInstinctAction.Icon,
                IconLabel = BasicInstinctAction.Name.ToString(),
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