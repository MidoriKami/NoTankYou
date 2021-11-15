using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Interface.Windowing;

namespace NoTankYou
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "No Tank You";

        private const string settingsCommand = "/notankconfig";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private SettingsWindow SettingsWindow { get; init; }
        private WarningWindow WarningWindow { get; init; }


        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            // If configuration json exists load it, if not make new config object
            Service.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(this.PluginInterface);

            // Load Tank Stance warning image
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var imagePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, "TankStance.png");
            var warningImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            // Create Windows
            this.SettingsWindow = new SettingsWindow();
            this.WarningWindow = new WarningWindow(warningImage);

            // Register Slash Commands
            this.CommandManager.AddHandler(settingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "No Tank You plugin options menu."
            });

            // Register draw callbacks
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

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
            this.SettingsWindow.IsOpen = true;
        }

        private void OnCommand(string command, string arguments)
        {
            this.SettingsWindow.IsOpen = true;
        }

        public void Dispose()
        {
            this.SettingsWindow.Dispose();
            this.WarningWindow.Dispose();  
            this.CommandManager.RemoveHandler(settingsCommand);
        }
    }
}
