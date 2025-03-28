﻿using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.DebugWindows;
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
        System.SystemConfig.UpdateCharacterData();
        System.SystemConfig.Save();
        
        System.BlacklistController.Load();
        System.ModuleController.Load();
        System.BannerController.Enable();
        System.PartyListController.Load();
    }
    
    private void OnLogout(int type, int code) {
        System.BlacklistController.Unload();
        System.ModuleController.Unload();
        System.BannerController.Disable();
        System.PartyListController.Unload();
    }
    
    private void OnZoneChange(ushort newZoneId) {
        if (!Service.ClientState.IsLoggedIn) return;

        System.ModuleController.ZoneChange(newZoneId);
    }
}