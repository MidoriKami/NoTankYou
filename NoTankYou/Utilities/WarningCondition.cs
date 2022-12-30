using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.System;
using Condition = KamiLib.Utilities.Condition;

namespace NoTankYou.Utilities;

internal static class WarningCondition
{
    private static readonly Stopwatch ConditionLockout = Stopwatch.StartNew();

    private static readonly List<ConditionFlag> LockoutFlags = new()
    {
        ConditionFlag.Jumping,
        ConditionFlag.Jumping61,
        ConditionFlag.BetweenAreas,
        ConditionFlag.BetweenAreas51,
    };
    
    public static bool ShouldShowWarnings()
    {
        var blacklist = Service.ConfigurationManager.CharacterConfiguration.Blacklist;

        if (!PartyListAddon.DataAvailable) return false;
        if (!GameUserInterface.Instance.IsVisible) return false;
        if (Service.ClientState.IsPvP) return false;
        if (Condition.IsCrossWorld()) return false;
        if (!SpecialConditions()) return false;
        if (blacklist.Enabled.Value && blacklist.ContainsCurrentZone()) return false;

        return true;
    }

    public static bool SpecialConditions()
    {
        // If any lockout flags are triggered
        if (LockoutFlags.Any(Condition.CheckFlag))
        {
            ConditionLockout.Restart();
            return false;
        }
        
        // Else, if we have not been locked out for 2 or more seconds
        else
        {
            return ConditionLockout.Elapsed >= TimeSpan.FromSeconds(2);
        }
    }
}