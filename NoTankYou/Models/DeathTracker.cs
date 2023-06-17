using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KamiLib.Windows;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Models;

public class DeathTracker
{
    private readonly Dictionary<uint, Stopwatch> deathStopwatch = new();

    public bool IsDead(IPlayerData playerData)
    {
        // If dead, add to dictionary and start a stopwatch
        if (playerData.IsDead()) deathStopwatch[playerData.GetObjectId()] = Stopwatch.StartNew();

        // If they are in the dictionary then they have recently died, or been dead, or have recently been revived
        if (deathStopwatch.TryGetValue(playerData.GetObjectId(), out var lastDeath))
        {
            // if its been less than 3 seconds since death, being dead, or revived, return they are dead
            if (lastDeath.Elapsed < TimeSpan.FromSeconds(3)) return true;
            
            // if its been more than 3 seconds, then they are definitely alive, remove from dictionary
            deathStopwatch.Remove(playerData.GetObjectId());
        }
        
        return false;
    }
}