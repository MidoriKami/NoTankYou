using System.Collections.Generic;

namespace NoTankYou.Classes;

public class SystemConfig {
    public int Version = 1;

    public HashSet<string> EnabledModules = [];
    
    public static SystemConfig Load()
        => Utilities.Config.LoadCharacterConfig<SystemConfig>("system.config.json");

    public void Save()
        => Utilities.Config.SaveCharacterConfig(this, "system.config.json");
}
