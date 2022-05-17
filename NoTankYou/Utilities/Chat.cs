using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;

namespace NoTankYou.Utilities
{
    internal static class Chat
    {
        public static void Print(string tag, string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[DailyDuty] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(62);
            stringBuilder.AddText($"[{tag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        public static void Debug(string message)
        {
            Print("Debug", message);
        }

        public static void Log(string tag, string message)
        {
            if (Service.Configuration.DeveloperMode)
            {
                PluginLog.Information(message);
                Print(tag, message);
            }
        }

        public static void Warning(string tag, string message)
        {
            if (Service.Configuration.DeveloperMode)
            {
                PluginLog.Warning(message);
                Print(tag, message);
            }
        }
    }
}