using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace NoTankYou.Classes;

public unsafe class WarningController {

    public readonly List<WarningInfo> ActiveWarnings = [];
    
    public void CollectWarnings(IEnumerable<ModuleBase>? modules) {
        ActiveWarnings.Clear();
        if (modules is null) return;

        if (SampleModeEnabled) {
            ActiveWarnings.AddRange(GetSampleWarnings());
        }

        foreach (var module in modules) {
            if (module.HasWarnings) {
                ActiveWarnings.AddRange(module.ActiveWarnings);
            }
        }

        ActiveWarnings.Sort((left, right) => right.Priority.CompareTo(left.Priority));
    }

    private static List<WarningInfo> GetSampleWarnings() {
        List<WarningInfo> warnings = [];

        warnings.AddRange(Enumerable.Range(0, 16)
            .Select(index => new WarningInfo {
                Message = $"NoTankYou Sample ({index + 1})",
                ActionId = 0,
                Priority = 100,
                IconId = 786,
                IconLabel = $"Sample Action Name ({index + 1})",
                SourceCharacter = CharacterManager.Instance()->BattleCharas[0], // Use LocalPlayer
                SourceModule = $"Sample Module ({index + 1})",
            })
        );

        return warnings;
    }
    
    public void ToggleSampleMode(bool newValue)
        => SampleModeEnabled = newValue;

    public bool SampleModeEnabled { get; private set; }
}
