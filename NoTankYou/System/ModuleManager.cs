﻿using System.Collections.Generic;
using System.Linq;
using KamiLib.Interfaces;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Modules;

namespace NoTankYou.System;

public class ModuleManager
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
        new Spiritbond(),
        new Cutscene(),
        new Chocobo(),
    };

    public ModuleManager()
    {
        foreach (var module in Modules)
        {
            module.RegisterCommand();
        }
    }

    public IEnumerable<ISelectable> GetConfigurationSelectables()
    {
        return Modules
            .OrderBy(item => item.Name.GetTranslatedString())
            .Select(module => module.ConfigurationComponent.Selectable);
    }

    public IEnumerable<ILogicComponent> GetModulesForClassJob(uint job)
    {
        return Modules
            .Where(module => module.LogicComponent.ClassJobs.Contains(job))
            .Select(module => module.LogicComponent);
    }
}