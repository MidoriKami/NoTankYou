using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Extensions;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.Modules
{
    internal class BlueMageModule : IModule
    {
        public List<uint> ClassJobs { get; }
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.BlueMage.GenericWarning;
        public string MessageShort => Strings.Modules.BlueMage.GenericWarning;
        public string ModuleCommand => "blu";
        private static BlueMageModuleSettings Settings => Service.Configuration.ModuleSettings.BlueMage;

        private readonly List<uint> MimicryStatusEffects;
        private readonly uint MightyGuardStatusEffect = 1719;
        private readonly uint BasicInstinct = 2498;
        private readonly uint AethericMimicryTank = 2124;

        private readonly Action MimicryAction;
        private readonly Action MightyGuardAction;
        private readonly Action BasicInstinctAction;

        public BlueMageModule()
        {
            ClassJobs = new List<uint> { 36 };

            MimicryStatusEffects = new List<uint>{ 2124, 2125, 2126 };

            MimicryAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(18322)!;
            MightyGuardAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(11417)!;
            BasicInstinctAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(23276)!;
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

            if (Settings.BasicInstinct && !character.HasStatus(BasicInstinct))
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
                Priority = GenericSettings.Priority,
                Sender = ModuleType.BlueMage,
                SenderSubtype = SenderSubtype.AetherialMimicry,
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
                Priority = GenericSettings.Priority,
                Sender = ModuleType.BlueMage,
                SenderSubtype = SenderSubtype.BasicInstinct,
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
                Priority = GenericSettings.Priority,
                Sender = ModuleType.BlueMage,
                SenderSubtype = SenderSubtype.MightyGuard,
            };
        }
    }
}
