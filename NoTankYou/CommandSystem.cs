using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using NoTankYou.SettingsSystem;
using System;

namespace NoTankYou
{
    internal class CommandSystem : IDisposable
    {

        private const string SettingsCommand = "/notankyou";
        private const string ShorthandCommand = "/nty";
        private const ushort ColorRed = 534;
        private const ushort ColorGreen = 45;

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

            if (primaryCommand == null)
            {
                SettingsWindow.IsOpen = !SettingsWindow.IsOpen;

                Service.Configuration.Save();
                return;
            }

            switch (primaryCommand.ToLower())
            {
                case "all":
                case "everything":
                    ProcessKardionCommands(secondaryCommand);
                    ProcessTankStanceCommands(secondaryCommand);
                    ProcessDancePartnerCommands(secondaryCommand);
                    ProcessFaerieCommands(secondaryCommand);
                    break;

                case "kardion":
                case "sage":
                case "sge":
                case "kardia":
                    ProcessKardionCommands(secondaryCommand);
                    break;

                case "tank":
                case "tankstance":
                    ProcessTankStanceCommands(secondaryCommand);
                    break;

                case "dancepartner":
                case "dancer":
                case "dp":
                case "partner":
                case "dnc":
                    ProcessDancePartnerCommands(secondaryCommand);
                    break;

                case "faerie":
                case "fairy":
                case "scholar":
                case "sch":
                    ProcessFaerieCommands(secondaryCommand);
                    break;

                case "mode":
                    ProcessModeCommands(secondaryCommand);
                    break;

                default:
                    break;
            }

            Service.Configuration.Save();
        }
        private void ProcessModeCommands(string? secondaryCommand)
        {
            if (secondaryCommand == null)
            {
                ToggleProcessingMainMode();
                PrintColoredConfigurationMode();
                return;
            }

            switch (secondaryCommand)
            {
                case "party:":
                    Service.Configuration.ProcessingMainMode = Configuration.MainMode.Party;
                    break;

                case "solo":
                case "trust":
                    Service.Configuration.ProcessingMainMode = Configuration.MainMode.Solo;
                    break;

                case "t":
                case "toggle":
                    ToggleProcessingMainMode();
                    break;
            }

            PrintColoredConfigurationMode();
        }
        private static void ProcessGenericOnOffToggleCommand(string? argument, ref bool booleanVariable, string message)
        {

            if (argument == null)
            {
                booleanVariable = !booleanVariable;
                PrintColoredStatus(message, booleanVariable);
                return;
            }

            switch (argument.ToLower())
            {
                case "on":
                    booleanVariable = true;
                    PrintColoredStatus(message, true);
                    break;

                case "off":
                    booleanVariable = false;
                    PrintColoredStatus(message, false);
                    break;

                case "toggle":
                case "t":
                case "tog":
                    booleanVariable = !booleanVariable;
                    PrintColoredStatus(message, booleanVariable);
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
        private static void PrintColoredStatus(string message, bool status)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddText($"{message}");

            if (status == true)
            {
                stringBuilder.AddUiForeground(ColorGreen);
                stringBuilder.AddText("enabled");
            }
            else
            {
                stringBuilder.AddUiForeground(ColorRed);
                stringBuilder.AddText("disabled");
            }
            stringBuilder.AddUiForegroundOff();

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        private static void PrintColoredConfigurationMode()
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddText($"[NoTankYou] Configuration Mode: ");

            if (Service.Configuration.ProcessingMainMode == Configuration.MainMode.Party)
            {
                stringBuilder.AddUiForeground(ColorGreen);
                stringBuilder.AddText("Party Mode");
                stringBuilder.AddUiForegroundOff();
                Service.Chat.Print(stringBuilder.BuiltString);
            }
            else if (Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo)
            {
                stringBuilder.AddUiForeground(ColorRed);
                stringBuilder.AddText("Solo Mode");
                stringBuilder.AddUiForegroundOff();
                Service.Chat.Print(stringBuilder.BuiltString);

                stringBuilder = new();
                stringBuilder.AddText("[NoTankYou] Configuration SubMode: ");

                if (Service.Configuration.ProcessingSubMode == Configuration.SubMode.OnlyInDuty)
                {
                    stringBuilder.AddUiForeground(514);
                    stringBuilder.AddText("Only in Duties");
                }
                else
                {
                    stringBuilder.AddUiForeground(500);
                    stringBuilder.AddText("Everywhere");
                }
                stringBuilder.AddUiForegroundOff();
                Service.Chat.Print(stringBuilder.BuiltString);
            }
        }

        private void ProcessFaerieCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.FaerieSettings.Enabled, "[NoTankYou] Faerie Warning: ");
        }

        private void ProcessDancePartnerCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.DancePartnerSettings.Enabled, "[NoTankYou] Dance Partner Warning: ");
        }

        private void ProcessTankStanceCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.TankStanceSettings.Enabled, "[NoTankYou] Tank Stance Warning: ");
        }

        private void ProcessKardionCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.KardionSettings.Enabled, "[NoTankYou] Kardion Warning: ");
        }

        private void ToggleProcessingMainMode()
        {
            if (Service.Configuration.ProcessingMainMode == Configuration.MainMode.Party)
            {
                Service.Configuration.ProcessingMainMode = Configuration.MainMode.Solo;
            }
            else if (Service.Configuration.ProcessingMainMode == Configuration.MainMode.Solo)
            {
                Service.Configuration.ProcessingMainMode = Configuration.MainMode.Party;
            }
        }
        public void Dispose()
        {
            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(ShorthandCommand);
        }
    }
}
