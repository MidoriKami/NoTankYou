using System.IO;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.Utilities;
using NoTankYou.Windows.NoTankYouWindow;
using Configuration = NoTankYou.Data.Configuration;

namespace NoTankYou
{
    public sealed class NoTankYouPlugin : IDalamudPlugin
    {
        public string Name => "NoTankYou";
        private const string SettingsCommand = "/nty";
        private const string HelpCommand = "/nty help";

        public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
        {
            // Create Static Services for use everywhere
            pluginInterface.Create<Service>();

            // Register Slash Commands
            Service.Commands.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "open configuration window"
            });

            Service.Commands.AddHandler(HelpCommand, new CommandInfo(OnCommand)
            {
                HelpMessage = "display a list of all available commands"
            });

            // If configuration json exists load it, if not make new config object
            Service.Configuration = Service.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            // Initialize Languages
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var filePath = Path.Combine(assemblyLocation, @"translations");

            Service.Localization = new Dalamud.Localization(filePath, "NoTankYou_");
            var dalamudLanguage = Service.PluginInterface.UiLanguage;
            LoadLocalization(dalamudLanguage);

            // Create Custom Services
            Service.ModuleManager = new ModuleManager();
            Service.ContextManager = new ContextManager();
            Service.FontManager = new FontManager();
            Service.IconManager = new IconManager();
            Service.HudManager = new HudManager();
            Service.EventManager = new EventManager();
            Service.WindowManager = new WindowManager();

            Service.Framework.Update += OnUpdate;

            // Register draw callbacks
            Service.PluginInterface.UiBuilder.Draw += Service.WindowSystem.Draw;
            Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            Service.PluginInterface.LanguageChanged += LoadLocalization;
        }

        private void OnUpdate(Framework framework)
        {
            Service.EventManager.Update();
            Service.ContextManager.Update();
            Service.HudManager.Update();
        }

        private static void LoadLocalization(string languageCode)
        {
            PluginLog.Information($"Loading Localization for {languageCode}");

            Service.Localization.SetupWithLangCode(languageCode);

            Strings.ReInitialize();
        }

        private void OnCommand(string command, string arguments)
        {
            Service.WindowManager.ExecuteCommand(command, arguments);
            Service.ModuleManager.ProcessCommand(command, arguments);

            switch (arguments)
            {
                case "help":
                    Chat.Print("Commands", Strings.Commands.Help);
                    break;

                case "coffee":
                    Chat.Print("Chad Mode", "Unable to brew, coffee printer out of ink.");
                    break;
#if DEBUG
                case "generateloc":
                    Chat.Debug("Generating Localization File");
                    Service.Localization.ExportLocalizable();
                    break;
#else
                case "generateloc":
                    Chat.Debug("Command not available in release mode");
                    break;
#endif
            }
        }

        private void DrawConfigUI() => Service.WindowManager.GetWindowOfType<NoTankYouWindow>()?.Toggle();

        public void Dispose()
        {
            Service.WindowManager.Dispose();
            Service.EventManager.Dispose();
            Service.HudManager.Dispose();
            Service.IconManager.Dispose();
            Service.FontManager.Dispose();

            Service.Framework.Update -= OnUpdate;

            Service.PluginInterface.UiBuilder.Draw -= Service.WindowSystem.Draw;
            Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            Service.PluginInterface.LanguageChanged -= LoadLocalization;

            Service.Commands.RemoveHandler(SettingsCommand);
            Service.Commands.RemoveHandler(HelpCommand);
        }
    }
}
