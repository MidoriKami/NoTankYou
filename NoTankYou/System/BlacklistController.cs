using KamiLib.AutomaticUserInterface;
using KamiLib.Utilities;
using NoTankYou.Models;

namespace NoTankYou.System;

public class BlacklistController
{
    private BlacklistConfig config = new();

    public void DrawConfig() => DrawableAttribute.DrawAttributes(config, SaveConfig);
    public bool IsZoneBlacklisted(uint zoneId) => config.BlacklistedZones.Contains(zoneId) && config.Enabled;
    public void Load() => config = LoadConfig();
    public void Unload() { }
    private BlacklistConfig LoadConfig() => CharacterFileController.LoadFile<BlacklistConfig>("Blacklist.config.json", config);
    public void SaveConfig() => CharacterFileController.SaveFile("Blacklist.config.json", config.GetType(), config);
}