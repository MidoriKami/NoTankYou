using Dalamud.Game.Command;
using System;
using Dalamud.Game.Gui;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace NoTankYou
{
    internal class CommandSystem : IDisposable
    {

        private const string settingsCommand = "/notankyou";
        private const string shorthandCommand = "/nty";
        private const ushort Color_Red = 534;
        private const ushort Color_Green = 45;
        
        private SettingsWindow settingsWindow;

        public CommandSystem(SettingsWindow settingsWindow)
        {
            RegisterCommands();

            this.settingsWindow = settingsWindow;
        }

        private void RegisterCommands()
        {
            // Register Slash Commands
            Service.Commands.AddHandler(settingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(shorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });
        }

        private void PrintColoredStatus(string message, bool status)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddText($"{message}");

            if( status == true )
            {
                stringBuilder.AddUiForeground(45);
                stringBuilder.AddText("enabled");
            }
            else
            {
                stringBuilder.AddUiForeground(534);
                stringBuilder.AddText("disabled");
            }
            stringBuilder.AddUiForegroundOff();

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        // 45 = green
        // 534 = red
        private void PrintStringColored(string message, ushort color)
        {
            var stringBuilder = new SeStringBuilder();

            stringBuilder.AddUiForeground(color);
            stringBuilder.AddText(message);
            stringBuilder.AddUiForegroundOff();

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        // Valid Command Structure:
        // /nty [main command] [on/off/nothing]
        private void OnCommand(string command, string arguments)
        {
            var primaryCommand = GetPrimaryCommand(arguments);
            var secondaryCommand = GetSecondaryCommand(arguments);

            if (primaryCommand == null)
            {
                settingsWindow.IsOpen = !settingsWindow.IsOpen;

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

                default:
                    break;
            }

            Service.Configuration.Save();
        }

        private string? GetSecondaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if (stringArray.Length == 1)
            {
                return null;
            }

            return stringArray[1];
        }

        private string? GetPrimaryCommand(string arguments)
        {
            var stringArray = arguments.Split(' ');

            if(stringArray[0] == string.Empty)
            {
                return null;
            }

            return stringArray[0];
        }

        private void ProcessGenericOnOffToggleCommand(string? argument, ref bool booleanVariable, string message)
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

        private void ProcessFaerieCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.EnableFaerieBanner, "[NoTankYou] Faerie Warning: ");
        }

        private void ProcessDancePartnerCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.EnableDancePartnerBanner, "[NoTankYou] Dance Partner Warning: ");
        }

        private void ProcessTankStanceCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.EnableTankStanceBanner, "[NoTankYou] Tank Stance Warning: ");
        }

        private void ProcessKardionCommands(string? secondaryCommand)
        {
            ProcessGenericOnOffToggleCommand(secondaryCommand, ref Service.Configuration.EnableKardionBanner, "[NoTankYou] Kardion Warning: ");
        }

        public void Dispose()
        {
            Service.Commands.RemoveHandler(settingsCommand);
            Service.Commands.RemoveHandler(shorthandCommand);
        }
    }
}
