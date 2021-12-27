using Dalamud.Game.Text.SeStringHandling;

namespace NoTankYou.CommandSystem.Commands
{
    internal static class ModeToggleCommand
    {
        private const ushort ColorRed = 534;
        private const ushort ColorGreen = 45;

        public static void ProcessCommand(string? secondaryCommand)
        {
            ProcessModeCommands(secondaryCommand);
        }

        private static void ProcessModeCommands(string? secondaryCommand)
        {
            switch (secondaryCommand)
            {
                case null:
                    ToggleProcessingMainMode();
                    break;

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

        private static void ToggleProcessingMainMode()
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
    }
}
