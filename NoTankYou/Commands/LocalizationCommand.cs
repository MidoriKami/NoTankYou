using System.Collections.Generic;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace NoTankYou.Commands;

internal class LocalizationCommand : IPluginCommand
{
    public string CommandArgument => "loc";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "generate",
            CommandAction = () =>
            {
                Service.LocalizationManager.ExportLocalization();
                Chat.Print("Command", "Generating Localization File");
            },
            Hidden = true,
        },
    };
}