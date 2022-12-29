using NoTankYou.DataModels;

namespace NoTankYou.Interfaces;

internal interface IModule
{
    GenericSettings GenericSettings { get; }

    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    ILogicComponent LogicComponent { get; }
}