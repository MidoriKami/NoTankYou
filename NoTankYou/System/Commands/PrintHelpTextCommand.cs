using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.System.Commands;

internal class PrintHelpTextCommand : IPluginCommand
{
    public string CommandArgument => "help";

    public void Execute(string? additionalArguments)
    {
        switch (additionalArguments)
        {
            case null:
                Chat.Print(Strings.Commands.Label ,Strings.Commands.Help);
                break;
        }
    }
}