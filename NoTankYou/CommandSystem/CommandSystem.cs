using System;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using NoTankYou.CommandSystem.Commands;
using NoTankYou.SettingsSystem;

namespace NoTankYou.CommandSystem
{
    internal class CommandSystem : IDisposable
    {

        private const string SettingsCommand = "/notankyou";
        private const string ShorthandCommand = "/nty";

        private readonly SettingsWindow SettingsWindow;

        public CommandSystem(SettingsWindow settingsWindow)
        {
            RegisterCommands();

            this.SettingsWindow = settingsWindow;
        }

        private void RegisterCommands()
        {
            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });
        }

        // Valid Command Structure:
        // /nty [main command] [on/off/nothing]
        private void OnCommand(string command, string arguments)
        {
            var primaryCommand = GetPrimaryCommand(arguments);
            var secondaryCommand = GetSecondaryCommand(arguments);

            switch (primaryCommand?.ToLower())
            {
                case null:
                    SettingsWindow.IsOpen = !SettingsWindow.IsOpen;
                    break;

                case "all":
                case "everything":
                    ModuleToggleCommand.ProcessCommand(ModuleToggleCommand.Modes.All, secondaryCommand);
                    break;

                case "kardion":
                case "sage":
                case "sge":
                case "kardia":
                    ModuleToggleCommand.ProcessCommand(ModuleToggleCommand.Modes.Kardion, secondaryCommand);
                    break;

                case "tank":
                case "tankstance":
                    ModuleToggleCommand.ProcessCommand(ModuleToggleCommand.Modes.TankStance, secondaryCommand);
                    break;

                case "dancepartner":
                case "dancer":
                case "dp":
                case "partner":
                case "dnc":
                    ModuleToggleCommand.ProcessCommand(ModuleToggleCommand.Modes.DancePartner, secondaryCommand);
                    break;

                case "faerie":
                case "fairy":
                case "scholar":
                case "sch":
                    ModuleToggleCommand.ProcessCommand(ModuleToggleCommand.Modes.Faerie, secondaryCommand);
                    break;

                case "mode":
                    ModeToggleCommand.ProcessCommand(secondaryCommand);
                    break;

                case "blacklist":
                case "bl":
                    BlacklistCommand.ProcessBlacklistCommand(secondaryCommand);
                    break;

                case "whitelist":
                case "wl":
                    BlacklistCommand.ProcessWhitelistCommand(secondaryCommand);
                    break;

                default:
                    break;
            }

            Service.Configuration.Save();
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

        public void Dispose()
        {
            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);
        }
    }
}
