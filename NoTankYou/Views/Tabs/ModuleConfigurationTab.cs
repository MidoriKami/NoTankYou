using System.Collections.Generic;
using System.Linq;
using KamiLib.Interfaces;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.Views.Components;

namespace NoTankYou.UserInterface.Tabs;

public class ModuleConfigurationTab : ISelectionWindowTab
{
    public string TabName => Strings.Modules;
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables() => NoTankYouSystem.ModuleController.Modules.Select(module => new ModuleSelectable(module));
}

