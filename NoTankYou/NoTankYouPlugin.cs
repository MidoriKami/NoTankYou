using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using NoTankYou.DisplaySystem;
using System.IO;

namespace NoTankYou
{
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "No Tank You";

        private SettingsWindow SettingsWindow { get; init; }
        private DisplayManager DisplayManager { get; init; }
        private CommandSystem CommandSystem { get; init; }

        public NoTankYouPlugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Service.Configuration.Initialize(Service.PluginInterface);

            // Load Tank Stance warning image
            var assemblyLocation = pluginInterface.AssemblyLocation.DirectoryName!;
            var dancePartnerPath = Path.Combine(assemblyLocation, @"images\DancePartner.png");
            var faeriePath = Path.Combine(assemblyLocation, @"images\Faerie.png");
            var kardionPath = Path.Combine(assemblyLocation, @"images\Kardion.png");
            var tankStancePath = Path.Combine(assemblyLocation, @"images\TankStance.png");

            var dancePartnerImage = Service.PluginInterface.UiBuilder.LoadImage(dancePartnerPath);
            var faerieImage = Service.PluginInterface.UiBuilder.LoadImage(faeriePath);
            var kardionImage = Service.PluginInterface.UiBuilder.LoadImage(kardionPath);
            var tankStanceImage = Service.PluginInterface.UiBuilder.LoadImage(tankStancePath);

            // Create Windows
            SettingsWindow = new SettingsWindow();
            DisplayManager = new DisplayManager(dancePartnerImage, faerieImage, kardionImage, tankStanceImage);

            // Register FrameworkUpdate
            Service.Framework.Update += OnFrameworkUpdate;

            // Create Command System
            CommandSystem = new CommandSystem(SettingsWindow);

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

        public void Dispose()
        {
            DisplayManager.Dispose();
            SettingsWindow.Dispose();
            CommandSystem.Dispose();
            Service.WindowSystem.RemoveAllWindows();
            Service.Framework.Update -= OnFrameworkUpdate;
        }
    }
}
