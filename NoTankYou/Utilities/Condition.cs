using Dalamud.Game.ClientState.Conditions;
using NoTankYou.Configuration.Components;
using NoTankYou.System;

namespace NoTankYou.Utilities;

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
        var blacklist = Service.ConfigurationManager.CharacterConfiguration.Blacklist;

        if (!PartyListAddon.DataAvailable) return false;
        if (!Service.ContextManager.ShowWarnings) return false;
        if (Service.ClientState.IsPvP) return false;
        if (Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance]) return false;
        if (blacklist.Enabled.Value && blacklist.ContainsCurrentZone()) return false;

        return true;
    }
}