using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiLib.Window;
using KamiToolKit;
using NoTankYou.Classes;
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
        System.WindowManager = new WindowManager(Service.PluginInterface);
        
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn);
        
        if (Service.ClientState.IsLoggedIn) {
            OnLogin();
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }
        
    public void Dispose() {
        System.LocalizationController.Dispose();
        
        System.ModuleController.Dispose();
        System.BannerController.Dispose();
        System.PartyListController.Dispose();
        
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        
        System.NativeController.Dispose();
    }
    
     private void OnFrameworkUpdate(IFramework framework) {
         if (Service.ClientState.IsPvP) {
             System.PartyListController.Hide();
             System.BannerController.Hide();
             return;
         }
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.Condition.IsBetweenAreas()) return;

        // Process and Collect Warnings
        System.ActiveWarnings = System.ModuleController.EvaluateWarnings();

        // Update time-step for animations
        System.PartyListController.Update();

        // Draw Warnings
        System.PartyListController.Draw(System.ActiveWarnings);
        System.BannerController.Draw(System.ActiveWarnings);
    }
    
    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.SystemConfig.UpdateCharacterData(Service.ClientState);
        System.SystemConfig.Save();
        
        if (System.SystemConfig.AutoSuppress) {
            Service.Log.Warning($"User Enabled AutoSuppression. Warnings will automatically be silenced after being active for {System.SystemConfig.AutoSuppressTime} seconds");
        }
        
        System.BlacklistController.Load();
        System.ModuleController.Load();
        System.BannerController.Load();
        System.PartyListController.Load();
    }
    
    private void OnLogout() {
        System.BlacklistController.Unload();
        System.ModuleController.Unload();
        System.BannerController.Unload();
        System.PartyListController.Unload();
    }
    
    private void OnZoneChange(ushort newZoneId) {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        System.ModuleController.ZoneChange(newZoneId);
    }
}