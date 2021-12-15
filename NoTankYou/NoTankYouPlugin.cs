using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using NoTankYou.DisplaySystem;
using System.IO;
using System.Reflection;

namespace NoTankYou
{
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "No Tank You";

        private const string settingsCommand = "/notankyou";
        private const string shorthandCommand = "/pnty";

        private SettingsWindow SettingsWindow { get; init; }
        private DisplayManager DisplayManager { get; init; }


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
            var dancePartnerPath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\DancePartner.png");
            var faeriePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\Faerie.png");
            var kardionPath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\Kardion.png");
            var tankStancePath = Path.Combine(Path.GetDirectoryName(assemblyLocation)!, @"images\TankStance.png");

            var dancePartnerImage = Service.PluginInterface.UiBuilder.LoadImage(dancePartnerPath);
            var faerieImage = Service.PluginInterface.UiBuilder.LoadImage(faeriePath);
            var kardionImage = Service.PluginInterface.UiBuilder.LoadImage(kardionPath);
            var tankStanceImage = Service.PluginInterface.UiBuilder.LoadImage(tankStancePath);

            // Create Windows
            SettingsWindow = new SettingsWindow();
            DisplayManager = new DisplayManager(dancePartnerImage, faerieImage, kardionImage, tankStanceImage);

            // Register FrameworkUpdate
            Service.Framework.Update += OnFrameworkUpdate;

            // Register Slash Commands
            Service.Commands.AddHandler(settingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(shorthandCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "shorthand command to open configuration window"
            });

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += DrawUI;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            // Register Windows
            Service.WindowSystem.AddWindow(SettingsWindow);

            Service.Chat.Enable();
        }
        private void OnFrameworkUpdate(Framework framework)
        {
            DisplayManager.Update();
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
            DisplayManager.Dispose();
            SettingsWindow.Dispose();
            Service.WindowSystem.RemoveAllWindows();
            Service.Commands.RemoveHandler(settingsCommand);
            Service.Commands.RemoveHandler(shorthandCommand);
            Service.Framework.Update -= OnFrameworkUpdate;
        }
    }
}
