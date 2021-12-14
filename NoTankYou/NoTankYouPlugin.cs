using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;

namespace NoTankYou
{
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "No Tank You";

        private const string settingsCommand = "/notankyou";

        private SettingsWindow SettingsWindow { get; init; }
        private WarningWindow WarningWindow { get; init; }


        public NoTankYouPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Load Tank Stance warning image
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var imagePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\TankStance.png");
            var warningImage = Service.PluginInterface.UiBuilder.LoadImage(imagePath);

            // Create Windows
            SettingsWindow = new SettingsWindow();
            WarningWindow = new WarningWindow(warningImage);

            // Register FrameworkUpdate
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Slash Commands
            Service.Commands.AddHandler(settingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            // Register Windows
            Service.WindowSystem.AddWindow(SettingsWindow);
            Service.WindowSystem.AddWindow(WarningWindow);

            Service.Chat.Enable();
        }
        private void OnFrameworkUpdate(Framework framework)
        {
            WarningWindow.Update();
        }

        private void DrawUI()
        {
            Service.WindowSystem.Draw();
        }
        private void DrawConfigUI()
        {
            SettingsWindow.IsOpen = true;
        }

        private void OnCommand(string command, string arguments)
        {
            switch (arguments)
            {
                default:
                    break;
            }

            SettingsWindow.IsOpen = true;

            Service.Configuration.Save();
        }

        public void Dispose()
        {
            WarningWindow.Dispose();
            SettingsWindow.Dispose();
            Service.Commands.RemoveHandler(settingsCommand);
            Service.Framework.Update -= OnFrameworkUpdate;
        }
    }
}
