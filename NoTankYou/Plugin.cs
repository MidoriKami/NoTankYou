using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;

namespace NoTankYou
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "No Tank You";

        private const string settingsCommand = "/notankconfig";

        private SettingsWindow SettingsWindow { get; init; }
        private WarningWindow WarningWindow { get; init; }


        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Load Tank Stance warning image
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var imagePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, "TankStance.png");
            var warningImage = Service.PluginInterface.UiBuilder.LoadImage(imagePath);

            // Create Windows
            SettingsWindow = new SettingsWindow();
            WarningWindow = new WarningWindow(warningImage);

            // Register Slash Commands
            Service.Commands.AddHandler(settingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "No Tank You plugin options menu."
            });

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            // Register Windows
            Service.WindowSystem = new WindowSystem("NoTankYou");

            Service.WindowSystem.AddWindow(SettingsWindow);
            Service.WindowSystem.AddWindow(WarningWindow);
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
            SettingsWindow.IsOpen = true;
        }

        public void Dispose()
        {
            SettingsWindow.Dispose();
            WarningWindow.Dispose();  
            Service.Commands.RemoveHandler(settingsCommand);
        }
    }
}
