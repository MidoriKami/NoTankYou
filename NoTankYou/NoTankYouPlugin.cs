using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Microsoft.VisualBasic;
using NoTankYou.Data;
using NoTankYou.System;
using NoTankYou.Windows;

namespace NoTankYou
{
    //D:\Documents\Visual Studio 2022\Repositories\Plugins\NoTankYou2\NoTankYou\NoTankYou.csproj
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "No Tank You";
        private const string SettingsCommand = "/nty";

        public NoTankYouPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.Chat.Enable();

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            // Create Custom Services
            Service.WindowManager = new WindowManager();

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void OnCommand(string command, string arguments) => Service.WindowManager.ExecuteCommand(command, arguments);

        private void DrawUI() => Service.WindowSystem.Draw();

        private void DrawConfigUI() => Service.WindowManager.GetWindowOfType<NoTankYouWindow>()?.Toggle();

        public void Dispose()
        {
            Service.WindowSystem.RemoveAllWindows();

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Service.Commands.RemoveHandler(SettingsCommand);
        }
    }
}
