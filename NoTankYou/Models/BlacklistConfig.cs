using System.Collections.Generic;
using KamiLib.AutomaticUserInterface;
using NoTankYou.Models.Attributes;

namespace NoTankYou.Models;

public class BlacklistConfig
{
    [BoolConfigOption("Enable", "Options", 0)]
    public bool Enabled = false;

    [Blacklist("Blacklist", 1)]
    public HashSet<uint> BlacklistedZones = new();
}