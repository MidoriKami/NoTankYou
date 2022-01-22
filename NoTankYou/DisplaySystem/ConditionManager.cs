using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace NoTankYou.DisplaySystem
{

    internal class ConditionManager
    {
        private readonly string ExitNodeText;

        public ConditionManager()
        {
            ExitNodeText = Service.DataManager.GetExcelSheet<EObjName>()!
                .GetRow(2000139)!
                .Singular;
        }
        public bool IsPartyMode()
        {
            var isInParty = Service.PartyList.Length > 0;
            var isBoundByDuty = IsBoundByDuty();
            var isPartyMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Party;
            var isUnableToUseActions = UnableToUseActions();
            var isDutyEnded = IsDutyEnded();

            return isInParty && isBoundByDuty && isPartyMode && !isUnableToUseActions && !isDutyEnded;
        }

        public bool IsSoloDutiesOnly()
        {
            var isSoloMainMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo;
            var isDutiesOnlySubMode = Service.Configuration.ProcessingSubMode == Configuration.SubMode.OnlyInDuty;
            var isBoundBuByDuty = IsBoundByDuty();
            var isUnableToUseActions = UnableToUseActions();
            var isDutyEnded = IsDutyEnded();

            return isSoloMainMode && isDutiesOnlySubMode && isBoundBuByDuty && !isUnableToUseActions && !isDutyEnded;
        }

        public bool IsSoloEverywhere()
        {
            var isSoloMainMode = Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo;
            var isEverywhereSubMode = Service.Configuration.ProcessingSubMode == Configuration.SubMode.Everywhere;
            var isUnableToUseActions = UnableToUseActions();

            return isSoloMainMode && isEverywhereSubMode && !isUnableToUseActions;
        }

        private bool IsDutyEnded()
        {
            return Service.ObjectTable
                .Any(o => o.ObjectKind == ObjectKind.EventObj && o.Name.ToString().ToLower() == ExitNodeText && WarningBanner.IsTargetable(o));
        }

        private bool IsBoundByDuty()
        {
            var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
            var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
            var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];

            // Triggers when Queue is started
            //var boundBy97 = Service.Condition[ConditionFlag.BoundToDuty97];

            return baseBoundByDuty || boundBy56 || boundBy95;
        }

        private bool UnableToUseActions()
        {
            var baseTransition = Service.Condition[ConditionFlag.BetweenAreas];
            var transition51 = Service.Condition[ConditionFlag.BetweenAreas51];
            var beingMoved = Service.Condition[ConditionFlag.BeingMoved];
            var jumping61 = Service.Condition[ConditionFlag.Jumping61];
            var mounted = Service.Condition[ConditionFlag.Mounted];
            var mounted2 = Service.Condition[ConditionFlag.Mounted2];

            return baseTransition || transition51 || beingMoved || jumping61 || mounted || mounted2;
        }
    }
}
