using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoTankYou.Classes;

public class SystemConfig {
    public int Version = 1;

    public HashSet<string> EnabledModules = [];

    public static async Task<SystemConfig> Load()
        => await Utilities.Config.LoadCharacterConfig<SystemConfig>("system.config.json");

    public async Task Save() {
        Services.PluginLog.Debug("Saving system.config.json");
        await Utilities.Config.SaveCharacterConfig(this, "system.config.json");
    }
}
