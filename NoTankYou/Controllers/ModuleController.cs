using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.Classes;
using NoTankYou.Classes;

namespace NoTankYou.Controllers;

public class ModuleController : IDisposable {
    public List<ModuleBase> Modules { get; } = [..Reflection.ActivateOfType<ModuleBase>()];

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
    
    public void Unload() {
        foreach (var module in Modules) {
            module.Unload();
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
    
    public void ZoneChange(ushort newZoneId) {
        foreach (var module in Modules) {
            module.ZoneChange(newZoneId);
        }
    }
}