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
        private const string shorthandCommand = "/nty";

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

        // Valid Command Structure:
        // /nty [main command] [on/off/nothing]
        private void OnCommand(string command, string arguments)
        {
            var argumentsArray = arguments.Split(' ');

            if(argumentsArray == null)
            {
                SettingsWindow.IsOpen = true;

                Service.Configuration.Save();

                return;
            }

            switch (argumentsArray[0].ToLower())
            {
                case "kardion":
                case "sage":
                case "sge":
                case "kardia":
                    ProcessKardionCommands(argumentsArray);
                    break;

                case "tank":
                case "tankstance":
                    ProcessTankStanceCommands(argumentsArray);
                    break;

                case "dancepartner":
                case "dancer":
                case "dp":
                case "partner":
                case "dnc":
                    ProcessDancePartnerCommands(argumentsArray);
                    break;

                case "faerie":
                case "fairy":
                case "scholar":
                case "sch":
                    ProcessFaerieCommands(argumentsArray);
                    break;

                default:
                    break;
            }

            Service.Configuration.Save();
        }

        private void ProcessGenericOnOffToggleCommand(string argument, ref bool booleanVariable)
        {
            if (argument == null)
            {
                booleanVariable = !booleanVariable;
                return;
            }

            switch (argument.ToLower())
            {
                case "on":
                    booleanVariable = true;
                    break;

                case "off":
                    booleanVariable = false;
                    break;

                case "toggle":
                case "t":
                case "tog":
                    booleanVariable = !booleanVariable;
                    break;
            }
        }

        private void ProcessFaerieCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableFaerieBanner);
        }

        private void ProcessDancePartnerCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableDancePartnerBanner);
        }

        private void ProcessTankStanceCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableTankStanceBanner);
        }

        private void ProcessKardionCommands(string[] argumentsArray)
        {
            ProcessGenericOnOffToggleCommand(argumentsArray[1], ref Service.Configuration.EnableKardionBanner);
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
