using System.Collections.Generic;
using System.Linq;
using NoTankYou.Interfaces;
using NoTankYou.Modules;

namespace NoTankYou.System;

internal class ModuleManager
{
    private IEnumerable<IModule> Modules { get; } = new List<IModule>()
    {
        new Tanks(),
        new BlueMage(),
        new Dancer(),
        new Food(),
        new FreeCompany(),
        new Sage(),
        new Scholar(),
        new Summoner(),
        new SpiritBond(),
    };

    public IEnumerable<ISelectable> GetConfigurationSelectables()
    {
        return Modules
            .Select(module => module.ConfigurationComponent.Selectable);
    }

    public IEnumerable<ILogicComponent> GetModulesForClassJob(uint job)
    {
        return Modules
            .Where(module => module.LogicComponent.ClassJobs.Contains(job))
            .Select(module => module.LogicComponent);
    }
}