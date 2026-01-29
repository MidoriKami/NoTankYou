using System;
using System.Collections.Generic;
using System.Diagnostics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace NoTankYou.Classes;

public unsafe class DeathTracker {
    private readonly Dictionary<uint, Stopwatch> deathStopwatch = new();

    public bool IsDead(BattleChara* playerData) {
        // If dead, add to dictionary and start a stopwatch
        if (playerData->IsDead()) deathStopwatch.TryAdd(playerData->EntityId, Stopwatch.StartNew());

        // If they are not in the dictionary, then they are alive.
        if (!deathStopwatch.TryGetValue(playerData->EntityId, out var lastDeath)) return false;

        // If it has been less than 5 seconds since death, being dead, or revived, return they are dead
        if (lastDeath.Elapsed < TimeSpan.FromSeconds(5)) return true;

        // If it has been more than 5 seconds, then they are definitely alive, remove from dictionary
        deathStopwatch.Remove(playerData->EntityId);

        return false;
    }
}
