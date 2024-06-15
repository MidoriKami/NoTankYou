using System.Collections.Generic;
using ImGuiNET;
using KamiLib.Configuration;

namespace NoTankYou.Controllers;

public class BlacklistController {
    private BlacklistConfig config = new();

    public void DrawConfig()
        => config.DrawConfigUi();
    
    public bool IsZoneBlacklisted(uint zoneId)
        => config.BlacklistedZones.Contains(zoneId) && config.Enabled;
    
    public void Load() 
        => config = LoadConfig();
    
    public void Unload() { }
    
    private BlacklistConfig LoadConfig() 
        => BlacklistConfig.Load();
}


public class BlacklistConfig {
    public bool Enabled { get; set; } = false;
    public HashSet<uint> BlacklistedZones { get; set; } = [];

    public void DrawConfigUi() {
        ImGui.Text("not implemented yet");
    }
    
    public static BlacklistConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "Blacklist.config.json", () => new BlacklistConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Blacklist.config.json", this);
}