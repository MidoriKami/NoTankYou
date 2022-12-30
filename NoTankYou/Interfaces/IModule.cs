using NoTankYou.DataModels;

namespace NoTankYou.Interfaces;

public interface IModule
{
    GenericSettings GenericSettings { get; }

    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    ILogicComponent LogicComponent { get; }
}