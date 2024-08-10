using System.Collections.Generic;
using KamiLib.Configuration;

namespace NoTankYou.Controllers;

public class BlacklistController {
    public BlacklistConfig config = new();

    public bool IsZoneBlacklisted(uint zoneId)
        => config.BlacklistedZones.Contains(zoneId);
    
    public void Load() 
        => config = LoadConfig();
    
    public void Unload() { }
    
    private BlacklistConfig LoadConfig() 
        => BlacklistConfig.Load();
}

public class BlacklistConfig {
    public HashSet<uint> BlacklistedZones { get; set; } = [];

    public static BlacklistConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "Blacklist.config.json", () => new BlacklistConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Blacklist.config.json", this);
}