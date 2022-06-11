using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using NoTankYou.Enums;

namespace NoTankYou.Utilities
{
    internal static class Chat
    {
        public static void Print(string tag, string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[NoTankYou] ");
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

        public static void Log(LogChannel channel, string message)
        {
            if (Service.Configuration.DeveloperMode)
            {
                switch (channel)
                {
                    case LogChannel.ContentDirector when Service.Configuration.DebugSettings.ContentDirector:
                        PluginLog.Information(message);
                        Print(channel.ToString(), message);
                        break;
                }
            }
        }
    }
}