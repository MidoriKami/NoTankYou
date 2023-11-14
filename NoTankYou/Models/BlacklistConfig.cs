using System.Collections.Generic;
using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models;

[Category("Options")]
public interface IBlacklistOptions
{
    [BoolConfig("Enable")]
    public bool Enabled { get; set; }
}

[Category("Blacklist",1)]
public interface IBlacklistZones
{
    [Blacklist]
    public HashSet<uint> BlacklistedZones { get; set; }
}

public class BlacklistConfig : IBlacklistOptions, IBlacklistZones
{
    public bool Enabled { get; set; } = false;
    public HashSet<uint> BlacklistedZones { get; set; } = new();
}