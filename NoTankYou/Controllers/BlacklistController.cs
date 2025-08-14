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
        => Service.PluginInterface.LoadCharacterFile<BlacklistConfig>(Service.ClientState.LocalContentId, "Blacklist.config.json");

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Blacklist.config.json", this);
}