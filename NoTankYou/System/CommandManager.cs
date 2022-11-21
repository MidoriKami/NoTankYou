using System;
using System.Collections.Generic;
using Dalamud.Game.Command;
using NoTankYou.Interfaces;
using NoTankYou.System.Commands;

namespace NoTankYou.System;

public class CommandManager : IDisposable
{
    private const string SettingsCommand = "/nty";
    private const string HelpCommand = "/nty help";

    private readonly List<IPluginCommand> Commands = new()
    {
        new ConfigurationWindowCommand(),
        new LocalizationCommand(),
        new DebugWindowCommand(),
    };

    public CommandManager()
    {
        Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "open configuration window"
        });

        Service.Commands.AddHandler(HelpCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = "display a list of all available sub-commands"
        });
    }

    public void Dispose()
    {
        Service.Commands.RemoveHandler(SettingsCommand);
        Service.Commands.RemoveHandler(HelpCommand);
    }

    private void OnCommand(string command, string arguments)
    {
        var subCommand = GetPrimaryCommand(arguments);
        var subCommandArguments = GetSecondaryCommand(arguments);

        switch (subCommand)
        {
            case null:
                Commands[0].Execute(subCommandArguments);
                break;

            case "help":
                Commands[1].Execute(subCommandArguments);
                break;
            
            default:
                foreach (var cmd in Commands)
                {
                    if (cmd.CommandArgument == subCommand)
                    {
                        cmd.Execute(subCommandArguments);
                    }
                }
                break;
        }
    }
    private static string? GetSecondaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray.Length == 1)
        {
            return null;
        }

        return stringArray[1];
    }

    private static string? GetPrimaryCommand(string arguments)
    {
        var stringArray = arguments.Split(' ');

        if (stringArray[0] == string.Empty)
        {
            return null;
        }

        return stringArray[0];
    }
}