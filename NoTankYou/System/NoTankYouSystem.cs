using System;
using System.Collections.Generic;
using DailyDuty.Models;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.GameFonts;
using Dalamud.Plugin.Services;
using KamiLib.AutomaticUserInterface;
using KamiLib.FileIO;
using NoTankYou.DataModels;
using NoTankYou.Utilities;

namespace NoTankYou.System;

public class NoTankYouSystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    public static BannerController BannerController = null!;
    public static PartyListController PartyListController = null!;
    public static GameFontHandle Axis56 = null!;
    public static SystemConfig SystemConfig = null!;
    public static BlacklistController BlacklistController = null!;
    private List<WarningState> activeWarnings = new();
    
    public NoTankYouSystem()
    {
        Axis56 = Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 56.0f));
        
        SystemConfig = new SystemConfig();
        
        LocalizationController.Instance.Initialize();

        BlacklistController = new BlacklistController();
        ModuleController = new ModuleController();
        BannerController = new BannerController();
        PartyListController = new PartyListController();
        
        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin();
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.PluginInterface.UiBuilder.Draw += OnDraw;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "_PartyList", OnPartyListUpdate);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", OnPartyListUpdate);
    }

    public void Dispose()
    {
        Axis56.Dispose();
        
        LocalizationController.Cleanup();
        ImageCache.Cleanup();
        
        ModuleController.Dispose();
        BannerController.Dispose();
        PartyListController.Dispose();
        
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.AddonLifecycle.UnregisterListener(OnPartyListUpdate);
    }

    public void DrawConfig() => DrawableAttribute.DrawAttributes(SystemConfig, SaveSystemConfig);

    private void OnFrameworkUpdate(IFramework framework)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        // Process and Collect Warnings
        activeWarnings = ModuleController.EvaluateWarnings();
    }
        
    private void OnPartyListUpdate(AddonEvent type, AddonArgs args) {
        PartyListController.Update();
        PartyListController.Draw(activeWarnings);
    }
    
    private void OnLogin()
    {
        LoadSystemConfig();
        if (SystemConfig.AutoSuppress) {
            Service.Log.Warning($"User Enabled AutoSuppression. Warnings will automatically be silenced after being active for {SystemConfig.AutoSuppressTime} seconds");
        }
        
        BlacklistController.Load();
        ModuleController.Load();
        BannerController.Load();
        PartyListController.Load();
    }
    
    private void OnLogout()
    {
        BlacklistController.Unload();
        ModuleController.Unload();
        BannerController.Unload();
        PartyListController.Unload();
    }
    
    private void OnDraw()
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        BannerController.Draw(activeWarnings);
    }
    
    private void OnZoneChange(ushort newZoneId)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        ModuleController.ZoneChange(newZoneId);
    }
    
    private void LoadSystemConfig()
    {
        SystemConfig = CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
        
        Service.Log.Debug($"[NoTankYouSystem] Logging into character: {Service.ClientState.LocalPlayer?.Name}, updating System.config.json");

        SystemConfig.CharacterName = Service.ClientState.LocalPlayer?.Name.ToString() ?? "Unable to Read Name";
        SystemConfig.CharacterWorld = Service.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString() ?? "Unable to Read World";
        SaveSystemConfig();
    }
    
    private void SaveSystemConfig() => CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}