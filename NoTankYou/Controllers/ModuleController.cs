using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.Classes;
using NoTankYou.Classes;

namespace NoTankYou.Controllers;

public class ModuleController : IDisposable {
    public List<ModuleBase> Modules { get; } = [..Reflection.ActivateOfType<ModuleBase>()];
    
    public static WarningState SampleWarning = new() {
        Message = "NoTankYou Sample Warning",
        ActionId = 0,
        Priority = 100,
        IconId = 786,
        IconLabel = "Sample Action",
        SourceEntityId = Service.ClientState.LocalPlayer?.EntityId ?? 0xE000000,
        SourcePlayerName = "Sample Player",
        SourceModule = ModuleName.Test,
    };

    public void Dispose() {
        foreach (var module in Modules.OfType<IDisposable>()) {
            module.Dispose();
        }
    }
    
    public void Load() {
        foreach (var module in Modules) {
            module.Load();
        }
    }
    
    public List<WarningState> EvaluateWarnings() {
        var warningList = new List<WarningState>();
        
        foreach (var module in Modules) {
            module.EvaluateWarnings();

            if (module.HasWarnings) {
                warningList.AddRange(module.ActiveWarningStates);
            }
        }

        return warningList;
    }
}