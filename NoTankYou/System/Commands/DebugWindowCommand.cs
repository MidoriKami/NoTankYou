using KamiLib.Utilities;
using NoTankYou.Interfaces;
using NoTankYou.UserInterface.Windows;

namespace NoTankYou.System.Commands;

internal class DebugWindowCommand : IPluginCommand
{
    public string CommandArgument => "debug";

    public void Execute(string? additionalArguments)
    {
        if ( Service.WindowManager.GetWindowOfType<DebugWindow>() is {} debugWindow )
        {
            if (Service.ClientState.IsPvP)
            {
                Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
            }
            else
            {
                debugWindow.IsOpen = !debugWindow.IsOpen;
            }
        }
    }
}