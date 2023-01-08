using KamiLib;
using NoTankYou.Commands;
using NoTankYou.DataModels;

namespace NoTankYou.Interfaces;

public interface IModule
{
    GenericSettings GenericSettings { get; }
    ModuleName Name { get; }
    IConfigurationComponent ConfigurationComponent { get; }
    ILogicComponent LogicComponent { get; }
    
    string Command { get; }

    void RegisterCommand()
    {
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand(Command, Name.GetTranslatedString(), () => GenericSettings));
    }
}
