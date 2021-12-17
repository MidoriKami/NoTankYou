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
            var test = new SeStringBuilder();
            test.AddText($"{message}");

            if( status == true )
            {
                test.AddUiForeground(45);
                test.AddText("enabled");
            }
            else
            {
                test.AddUiForeground(534);
                test.AddText("disabled");
            }
            test.AddUiForegroundOff();

            Service.Chat.Print(test.BuiltString);
        }

        // Valid Command Structure:
        // /nty [main command] [on/off/nothing]
        private void OnCommand(string command, string arguments)
        {
            var argumentsArray = arguments.Split(' ');


            if (argumentsArray == null)
            {
                settingsWindow.IsOpen = true;

                Service.Configuration.Save();

                return;
            }

            switch (argumentsArray[0].ToLower())
            {
                case "kardion":
                case "sage":
                case "sge":
                case "kardia":
                    ProcessKardionCommands(argumentsArray);
                    break;

                case "tank":
                case "tankstance":
                    ProcessTankStanceCommands(argumentsArray);
                    break;

                case "dancepartner":
                case "dancer":
                case "dp":
                case "partner":
                case "dnc":
                    ProcessDancePartnerCommands(argumentsArray);
                    break;

                case "faerie":
                case "fairy":
                case "scholar":
                case "sch":
                    ProcessFaerieCommands(argumentsArray);
                    break;

                default:
                    break;
            }

            Service.Configuration.Save();
        }

        private void ProcessGenericOnOffToggleCommand(string argument, ref bool booleanVariable, string message)
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

        private void ProcessFaerieCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableFaerieBanner, "[NoTankYou] Faerie Warning: ");
        }

        private void ProcessDancePartnerCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableDancePartnerBanner, "[NoTankYou] Dance Partner Warning: ");
        }

        private void ProcessTankStanceCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableTankStanceBanner, "[NoTankYou] Tank Stance Warning: ");
        }

        private void ProcessKardionCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableKardionBanner, "[NoTankYou] Kardion Warning: ");
        }

        public void Dispose()
        {
            Service.Commands.RemoveHandler(settingsCommand);
            Service.Commands.RemoveHandler(shorthandCommand);
        }
    }
}
