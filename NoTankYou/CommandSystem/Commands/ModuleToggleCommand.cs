using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;

namespace NoTankYou.CommandSystem.Commands
{
    internal static class ModuleToggleCommand
    {
        public enum Modes
        {
            DancePartner,
            Faerie,
            Kardion,
            Summoner,
            TankStance,
            BlueMage,
            All
        }

        private const ushort ColorRed = 534;
        private const ushort ColorGreen = 45;

        public static void ProcessCommand(Modes mode, string? secondaryCommand)
        {
            if (mode == Modes.All)
            {
                ToggleAll(secondaryCommand);
                return;
            }

            var settings = mode switch
            {
                Modes.TankStance => Service.Configuration.TankStanceSettings,
                Modes.DancePartner => Service.Configuration.DancePartnerSettings,
                Modes.Faerie => Service.Configuration.FaerieSettings,
                Modes.Kardion => Service.Configuration.KardionSettings,
                Modes.Summoner => Service.Configuration.SummonerSettings,
                Modes.BlueMage => Service.Configuration.BlueMageSettings,
                _ => null
            };

            if (settings == null)
            {
                Service.Chat.Print("[NoTankYou] Invalid Mode Selected in ModuleToggleCommand.ProcessCommand. Please report to plugin author.");
                return;
            }

            ToggleOne(secondaryCommand, settings, mode);
        }

        private static void ToggleOne(string? secondaryCommand, Configuration.ModuleSettings settings, Modes mode)
        {
            var message = GetMessageForMode(mode);

            switch (secondaryCommand?.ToLower())
            {
                case null:
                    settings.Enabled = !settings.Enabled;
                    PrintColoredStatus(message, settings.Enabled);
                    break;

                case "on":
                    settings.Enabled = true;
                    PrintColoredStatus(message, true);
                    break;

                case "off":
                    settings.Enabled = false;
                    PrintColoredStatus(message, false);
                    break;

                case "toggle":
                case "t":
                case "tog":
                    settings.Enabled = !settings.Enabled;
                    PrintColoredStatus(message, settings.Enabled);
                    break;
            }
        }

        private static void ToggleAll(string? secondaryCommand)
        {
            ToggleOne(secondaryCommand, Service.Configuration.DancePartnerSettings, Modes.DancePartner);
            ToggleOne(secondaryCommand, Service.Configuration.FaerieSettings, Modes.Faerie);
            ToggleOne(secondaryCommand, Service.Configuration.KardionSettings, Modes.Kardion);
            ToggleOne(secondaryCommand, Service.Configuration.SummonerSettings, Modes.Summoner);
            ToggleOne(secondaryCommand, Service.Configuration.TankStanceSettings, Modes.TankStance);
            ToggleOne(secondaryCommand, Service.Configuration.BlueMageSettings, Modes.BlueMage);
        }

        private static string GetMessageForMode(Modes mode)
        {
            return mode switch
            {
                Modes.DancePartner => "[NoTankYou] Dance Partner Warning: ",
                Modes.Faerie => "[NoTankYou] Faerie Warning: ",
                Modes.Kardion => "[NoTankYou] Kardion Warning: ",
                Modes.Summoner => "[NoTankYou] Summoner Pet Warning: ",
                Modes.TankStance => "[NoTankYou] Tank Stance Warning: ",
                Modes.BlueMage => "[NoTankYou] BlueMage Stance Warning: ",
                _ => "[NoTankYou] Invalid Mode. Please report this to plugin author."
            };
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
    }
}
