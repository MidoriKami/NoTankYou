using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.Enums;

namespace NoTankYou.Interfaces;

internal interface IModule
{
    GenericSettings GenericSettings { get; }

    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    ILogicComponent LogicComponent { get; }
}