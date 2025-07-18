using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.DebugWindows;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiToolKit;
using NoTankYou.Configuration;
using NoTankYou.Controllers;
using NoTankYou.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin {
    public NoTankYouPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        System.NativeController = new NativeController(Service.PluginInterface);

        System.SystemConfig = new SystemConfig();
        System.LocalizationController = new LocalizationController();
        System.CommandManager = new CommandManager(Service.PluginInterface, "notankyou", "nty");

        System.BlacklistController = new BlacklistController();
        System.ModuleController = new ModuleController();
        System.BannerController = new BannerController();
        System.PartyListController = new PartyListController();

        System.ConfigurationWindow = new ConfigurationWindow();
        System.DutyTypeDebugWindow = new DutyTypeDebugWindow(Service.DataManager);
        System.WindowManager = new WindowManager(Service.PluginInterface);
        
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn);
        System.WindowManager.AddWindow(System.DutyTypeDebugWindow);
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = _ => System.DutyTypeDebugWindow.UnCollapseOrToggle(),
            ActivationPath = "/dutytypedebug",
        });
        
        if (Service.ClientState.IsLoggedIn) {
            Service.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
    }

    public void Dispose() {
        System.LocalizationController.Dispose();

        System.ModuleController.Dispose();
        System.BannerController.Dispose();
        System.PartyListController.Dispose();

        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;

        System.NativeController.Dispose();
    }
    
     private void OnFrameworkUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.Condition.IsBetweenAreas()) return;

        // Process and Collect Warnings
        System.ActiveWarnings = System.ModuleController.EvaluateWarnings();

        // Draw Warnings
        System.PartyListController.UpdateWarnings(System.ActiveWarnings);
        System.BannerController.UpdateWarnings(System.ActiveWarnings);
    }
    
    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.SystemConfig.UpdateCharacterData();
        System.SystemConfig.Save();

        System.BlacklistController.Load();
        System.ModuleController.Load();
        System.BannerController.Enable();
        System.PartyListController.Enable();
    }
    
    private void OnLogout(int type, int code) {
        System.BannerController.Disable();
        System.PartyListController.Disable();
    }
}