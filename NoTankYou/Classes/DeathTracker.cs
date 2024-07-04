using System;
using System.Collections.Generic;
using System.Diagnostics;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Classes;

public class DeathTracker {
    private readonly Dictionary<uint, Stopwatch> deathStopwatch = new();

    public bool IsDead(IPlayerData playerData) {
        // If dead, add to dictionary and start a stopwatch
        if (playerData.IsDead()) deathStopwatch[playerData.GetEntityId()] = Stopwatch.StartNew();

        // If they are in the dictionary then they have recently died, or been dead, or have recently been revived
        if (deathStopwatch.TryGetValue(playerData.GetEntityId(), out var lastDeath)) {
            // if its been less than 5 seconds since death, being dead, or revived, return they are dead
            if (lastDeath.Elapsed < TimeSpan.FromSeconds(5)) return true;
            
            // if its been more than 5 seconds, then they are definitely alive, remove from dictionary
            deathStopwatch.Remove(playerData.GetEntityId());
        }
        
        return false;
    }
}