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
        pluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(pluginInterface);

        System.SystemConfig = new SystemConfig();
        System.CommandManager = new CommandManager(Services.PluginInterface, "notankyou", "nty");

        System.BlacklistController = new BlacklistController();
        System.ModuleController = new ModuleController();
        System.BannerController = new BannerController();
        System.PartyListController = new PartyListController();

        System.ConfigurationWindow = new ConfigurationWindow();
        System.DutyTypeDebugWindow = new DutyTypeDebugWindow(Services.DataManager);
        System.WindowManager = new WindowManager(Services.PluginInterface);
        
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn);
        System.WindowManager.AddWindow(System.DutyTypeDebugWindow);
        
        System.CommandManager.RegisterCommand(new CommandHandler {
            Delegate = _ => System.DutyTypeDebugWindow.UnCollapseOrToggle(),
            ActivationPath = "/dutytypedebug",
        });
        
        if (Services.ClientState.IsLoggedIn) {
            Services.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Services.Framework.Update += OnFrameworkUpdate;
        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;
    }

    public void Dispose() {
        System.ModuleController.Dispose();
        System.BannerController.Dispose();
        System.PartyListController.Dispose();

        Services.Framework.Update -= OnFrameworkUpdate;
        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;

        KamiToolKitLibrary.Dispose();
    }
    
     private void OnFrameworkUpdate(IFramework framework) {
        if (!Services.ClientState.IsLoggedIn) return;
        if (Services.Condition.IsBetweenAreas()) return;

        // Process and Collect Warnings
        System.ActiveWarnings = System.ModuleController.EvaluateWarnings();

        // Draw Warnings
        System.PartyListController.UpdateWarnings(System.ActiveWarnings);
        System.BannerController.UpdateWarnings(System.ActiveWarnings);
    }

     private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.BannerConfig = BannerConfig.Load();
        System.BannerListStyle = BannerListStyle.Load();
        System.BannerStyle = BannerStyle.Load();

        System.BlacklistController.Load();
        System.ModuleController.Load();
        System.PartyListController.Enable();
    }
    
    private void OnLogout(int type, int code) {
        System.SystemConfig = null;
        System.BannerConfig = null;
        System.BannerListStyle = null;
        System.BannerStyle = null;
        
        System.PartyListController.Disable();
    }
}