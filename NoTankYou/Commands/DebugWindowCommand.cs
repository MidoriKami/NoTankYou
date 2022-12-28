using System.Collections.Generic;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using NoTankYou.Windows;

namespace NoTankYou.Commands;

internal class DebugWindowCommand : IPluginCommand
{
    public string CommandArgument => "debug";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "open",
            CanExecute = () => Service.ClientState.IsPvP,
            CommandAction = () => Chat.PrintError("The configuration menu cannot be opened while in a PvP area"),
            GetHelpText = () => "Open Debug Window",
            Hidden = true,
        },
        new SubCommand
        {
            CommandKeyword = "open",
            CanExecute = () => !Service.ClientState.IsPvP,
            CommandAction = () =>
            {
                if (KamiLib.KamiLib.WindowManager.GetWindowOfType<DebugWindow>() is { } window)
                {
                    window.IsOpen = !window.IsOpen;
                }
            },
            GetHelpText = () => "Open Debug Window",
            Hidden = true,
        },
    };
}