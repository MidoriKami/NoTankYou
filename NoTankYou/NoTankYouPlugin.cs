using CheapLoc;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using NoTankYou.Data;
using NoTankYou.System;
using NoTankYou.Windows.NoTankYouWindow;

namespace NoTankYou
{
    //D:\Documents\Visual Studio 2022\Repositories\Plugins\NoTankYou2\NoTankYou\NoTankYou.csproj
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "NoTankYou";
        private const string SettingsCommand = "/nty";

        public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();
            Service.Chat.Enable();

            Loc.SetupWithFallbacks();

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            // Create Custom Services
            Service.ModuleManager = new ModuleManager();
            Service.WindowManager = new WindowManager();
            Service.EventManager = new EventManager();
            Service.HudManager = new HudManager();

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void OnCommand(string command, string arguments) => Service.WindowManager.ExecuteCommand(command, arguments);

        private void DrawUI() => Service.WindowSystem.Draw();

        private void DrawConfigUI() => Service.WindowManager.GetWindowOfType<NoTankYouWindow>()?.Toggle();

        public void Dispose()
        {
            Service.WindowManager.Dispose();
            Service.EventManager.Dispose();
            Service.HudManager.Dispose();

            Service.PluginInterface.UiBuilder.Draw -= DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

            Service.Commands.RemoveHandler(SettingsCommand);
        }
    }
}
