using System;
using System.Collections.Generic;
using DailyDuty.Models;
using Dalamud.Game;
using Dalamud.Interface.GameFonts;
using Dalamud.Logging;
using KamiLib.AutomaticUserInterface;
using NoTankYou.DataModels;
using NoTankYou.Utilities;

namespace NoTankYou.System;

public class NoTankYouSystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static BannerController BannerController = null!;
    public static GameFontHandle Axis56 = null!;
    public static SystemConfig SystemConfig = null!;
    private List<WarningState> activeWarnings = new();

    public NoTankYouSystem()
    {
        Axis56 = Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 56.0f));
        
        SystemConfig = new SystemConfig();
        
        LocalizationController.Instance.Initialize();
        
        ModuleController = new ModuleController();
        BannerController = new BannerController();
        
        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.EnterPvP += OnEnterPvP;
        Service.ClientState.LeavePvP += OnLeavePvP;
        Service.PluginInterface.UiBuilder.Draw += OnDraw;
    }
    
    public void Dispose()
    {
        Axis56.Dispose();
        
        LocalizationController.Cleanup();
        ImageCache.Cleanup();
        
        ModuleController.Dispose();
        
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.EnterPvP -= OnEnterPvP;
        Service.ClientState.LeavePvP -= OnLeavePvP;
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;
    }

    public void DrawConfig() => DrawableAttribute.DrawAttributes(SystemConfig, SaveSystemConfig);

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        // Process and Collect Warnings
        activeWarnings = ModuleController.EvaluateWarnings();
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        LoadSystemConfig();
        
        ModuleController.LoadModules();
        BannerController.Load();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        ModuleController.UnloadModules();
        BannerController.Unload();
    }
    
    private void OnDraw()
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        BannerController.Draw(activeWarnings);
    }
    
    private void OnLeavePvP()
    {
        
    }

    private void OnEnterPvP()
    {
        
    }
    
    private void LoadSystemConfig()
    {
        SystemConfig = FileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
        
        PluginLog.Debug($"[NoTankYouSystem] Logging into character: {Service.ClientState.LocalPlayer?.Name}, updating System.config.json");

        SystemConfig.CharacterName = Service.ClientState.LocalPlayer?.Name.ToString() ?? "Unable to Read Name";
        SystemConfig.CharacterWorld = Service.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString() ?? "Unable to Read World";
        SaveSystemConfig();
    }
    
    private void SaveSystemConfig() => FileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}