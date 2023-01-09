using System;
using System.Collections.Generic;
using Dalamud.Utility;
using KamiLib.ChatCommands;
using KamiLib.Interfaces;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.Commands;

public class GenericModuleCommand : IPluginCommand
{
    public string? CommandArgument { get; }
    public IEnumerable<ISubCommand> SubCommands { get; }

    public GenericModuleCommand(string command, string moduleName, Func<GenericSettings> getSettings)
    {
        CommandArgument = command;

        SubCommands = new List<ISubCommand>
        {
            new SubCommand
            {
                CommandKeyword = "enable",
                Aliases = new List<string>{"on"},
                CommandAction = () =>
                {
                    getSettings.Invoke().Enabled.Value = true;
                    Chat.Print(Strings.Common_Command, string.Format(Strings.Commands_EnablingModule, moduleName));
                },
                GetHelpText = () => string.Format(Strings.Commands_EnablesModule, moduleName),
            },
            new SubCommand
            {
                CommandKeyword = "disable",
                Aliases = new List<string>{"off"},
                CommandAction = () =>
                {
                    getSettings.Invoke().Enabled.Value = false;
                    Chat.Print(Strings.Common_Command, string.Format(Strings.Commands_DisablingModule, moduleName));
                },
                GetHelpText = () => string.Format(Strings.Commands_DisablesModule, moduleName),
            },
            new SubCommand
            {
                CommandKeyword = "toggle",
                Aliases = new List<string>{"t"},
                CommandAction = () =>
                {
                    getSettings.Invoke().Enabled.Value = !getSettings.Invoke().Enabled.Value;
                    var message = getSettings.Invoke().Enabled.Value ? Strings.Commands_EnablingModule.Format(moduleName) : Strings.Commands_DisablingModule.Format(moduleName);
                    
                    Chat.Print(Strings.Common_Command, message);
                },
                GetHelpText = () => string.Format(Strings.Commands_TogglesModule, moduleName),
            },
        };
    }
}