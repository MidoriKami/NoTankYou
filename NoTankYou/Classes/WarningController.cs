using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace NoTankYou.Classes;

public unsafe class WarningController : IDisposable {

    public readonly List<WarningInfo> ActiveWarnings = [];
    
    public void CollectWarnings(IEnumerable<ModuleBase>? modules) {
        ActiveWarnings.Clear();
        if (modules is null) return;

        if (SampleModeEnabled) {
            ActiveWarnings.Add(GetSampleWarning());
        }

        foreach (var module in modules) {
            if (module.HasWarnings) {
                ActiveWarnings.AddRange(module.ActiveWarnings);
            }
        }

        ActiveWarnings.Sort((left, right) => right.Priority.CompareTo(left.Priority));
    }

    private static WarningInfo GetSampleWarning() => new() {
        Message = "NoTankYou Sample Warning",
        ActionId = 0,
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action Name",
        SourceCharacter = CharacterManager.Instance()->BattleCharas[0], // Use LocalPlayer
        SourceModule = "Sample Module",
    };

    public void ToggleSampleMode(bool newValue)
        => SampleModeEnabled = newValue;

    public bool SampleModeEnabled { get; private set; }

    public void Dispose() {
        
    }
}
