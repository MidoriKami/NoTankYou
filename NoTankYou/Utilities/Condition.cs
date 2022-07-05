using Dalamud.Game.ClientState.Conditions;
using NoTankYou.Data.Components;
using NoTankYou.System;

namespace NoTankYou.Utilities
{
    internal static class Condition
    {
        public static bool IsBoundByDuty()
        {
            var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
            var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
            var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];
            
            return baseBoundByDuty || boundBy56 || boundBy95;
        }

        public static bool ShouldShowWarnings()
        {
            var hudDataAvailable = HudManager.DataAvailable;
            var showWarnings = Service.ContextManager.ShowWarnings;
            var isPvP = Service.ClientState.IsPvP;

            var blackListEnabled = Service.Configuration.SystemSettings.Blacklist.Enabled;
            var blackListedZone = Service.Configuration.SystemSettings.Blacklist.ContainsCurrentZone();
            var blacklisted = blackListEnabled && blackListedZone;

            var inCrossWorldParty = Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];

            return hudDataAvailable && showWarnings && !isPvP && !blacklisted && !inCrossWorldParty;
        }
    }
}