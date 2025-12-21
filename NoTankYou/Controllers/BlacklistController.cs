using System.Collections.Generic;
using KamiLib.Configuration;

namespace NoTankYou.Controllers;

public class BlacklistController {
    public BlacklistConfig Config = new();

    public bool IsZoneBlacklisted(uint zoneId)
        => Config.BlacklistedZones.Contains(zoneId);
    
    public void Load() 
        => Config = LoadConfig();
    
    private BlacklistConfig LoadConfig() 
        => BlacklistConfig.Load();
}

public class BlacklistConfig {
    public HashSet<uint> BlacklistedZones { get; set; } = [];

    public static BlacklistConfig Load() 
        => Services.PluginInterface.LoadCharacterFile<BlacklistConfig>(Services.PlayerState.ContentId, "Blacklist.config.json");

    public void Save()
        => Services.PluginInterface.SaveCharacterFile(Services.PlayerState.ContentId, "Blacklist.config.json", this);
}