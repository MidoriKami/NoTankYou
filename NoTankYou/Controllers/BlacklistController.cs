using System.Collections.Generic;

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
        => Utilities.Config.LoadCharacterConfig<BlacklistConfig>("Blacklist.config.json");

    public void Save()
        => Utilities.Config.SaveCharacterConfig(this, "Blacklist.config.json");
}