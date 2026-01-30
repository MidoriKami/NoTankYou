using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using KamiToolKit;
using NoTankYou.Classes;
using NoTankYou.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin {
    public NoTankYouPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(pluginInterface, "NoTankYou");
        
        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "NoTankYouConfig",
            Title = "NoTankYou Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };
        
        Services.CommandManager.AddHandler([ "/notankyou", "/nty" ], new CommandInfo(OnCommand) {
            HelpMessage = "Open NoTankYou config window",
        });
        
        System.WarningController = new WarningController();
        System.ModuleManager = new ModuleManager();
        
        if (Services.ClientState.IsLoggedIn) {
            Services.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;

        Services.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi += System.ConfigurationWindow.Toggle;
    }

    public void Dispose() {
        Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;

        System.ConfigurationWindow.Dispose();

        System.ModuleManager.Dispose();
        
        KamiToolKitLibrary.Dispose();
    }
    
    private static void OnCommand(string command, string arguments) {
        if (command is not ("/notankyou" or "/nty")) return;

        switch (arguments) {
            case null or "":
                if (!Services.ClientState.IsLoggedIn) return;
                System.ConfigurationWindow.Open();
                break;
        }
    }

    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.ModuleManager.LoadModules();
    }
    
    private void OnLogout(int type, int code) {
        System.ModuleManager.UnloadModules();
        System.SystemConfig = null;
    }
}
