using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using NoTankYou.Configuration.Components;
using NoTankYou.System;

namespace NoTankYou.Utilities;

internal static class Condition
{
    public static readonly Stopwatch ConditionLockout = new();

    private static readonly List<ConditionFlag> LockoutFlags = new()
    {
        ConditionFlag.Jumping,
        ConditionFlag.Jumping61,
        ConditionFlag.BetweenAreas,
        ConditionFlag.BetweenAreas51
    };
    
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
        if (!SpecialConditions()) return false;
        if (blacklist.Enabled.Value && blacklist.ContainsCurrentZone()) return false;

        return true;
    }

    public static bool SpecialConditions()
    {
        // If any lockout flags are triggered
        if (LockoutFlags.Any(flag => Service.Condition[flag]))
        {
            ConditionLockout.Restart();
            return false;
        }
        
        // Else, if we have not been locked out for 2 or more seconds
        else
        {
            return ConditionLockout.Elapsed.Seconds >= 2 || !ConditionLockout.IsRunning;
        }
    }
}