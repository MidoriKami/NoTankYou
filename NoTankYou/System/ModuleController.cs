using System;
using System.Collections.Generic;
using System.Linq;
using KamiLib.Utilities;
using NoTankYou.Abstracts;
using NoTankYou.DataModels;

namespace NoTankYou.System;

public class ModuleController : IDisposable
{
    public List<ModuleBase> Modules { get; }
    
    public ModuleController()
    {
        Modules = new List<ModuleBase>(Reflection.ActivateOfType<ModuleBase>());
    }
    
    public void Dispose()
    {
        foreach (var module in Modules.OfType<IDisposable>())
        {
            module.Dispose();
        }
    }
    
    public void Load()
    {
        foreach (var module in Modules)
        {
            module.Load();
        }
    }
    
    public void Unload()
    {
        foreach (var module in Modules)
        {
            module.Unload();
        }
    }
    
    public List<WarningState> EvaluateWarnings()
    {
        var warningList = new List<WarningState>();
        
        foreach (var module in Modules)
        {
            module.EvaluateWarnings();

            if (module.HasWarnings)
            {
                warningList.AddRange(module.ActiveWarningStates);
            }
        }

        return warningList;
    }
}