using System.Collections.Generic;
using System.Diagnostics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.GameState;
using NoTankYou.DataModels;

namespace NoTankYou.Interfaces;

public interface ILogicComponent
{
    IModule ParentModule { get; }
    List<uint> ClassJobs { get; }

    private static readonly Dictionary<uint, Stopwatch> DeathStopwatch = new();

    WarningState? EvaluateWarning(PlayerCharacter character);
    
    WarningState? ShouldShowWarning(PlayerCharacter character)
    {
        if (!ParentModule.GenericSettings.Enabled) return null;
        if (ParentModule.GenericSettings.DutiesOnly && !DutyState.Instance.IsDutyStarted) return null;
        if (ParentModule.GenericSettings.SoloMode && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return null;

        if (character.IsDead) DeathStopwatch[character.ObjectId] = Stopwatch.StartNew();
        if (DeathStopwatch.TryGetValue(character.ObjectId, out var lastDeath) && lastDeath.Elapsed.TotalSeconds < 3)
        {
            return null;
        }
        
        return EvaluateWarning(character);
    }
}