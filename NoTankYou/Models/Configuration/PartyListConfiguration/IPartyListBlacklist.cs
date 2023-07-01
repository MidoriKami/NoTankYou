using System.Collections.Generic;
using KamiLib.AutomaticUserInterface;
using NoTankYou.Models.Attributes;
using NoTankYou.Models.Enums;

namespace NoTankYou.Models.BannerConfiguration;

[Category("ModuleBlacklist")]
public interface IPartyListBlacklist
{
    [ModuleBlacklist("ModuleBlacklist")]
    public HashSet<ModuleName> BlacklistedModules { get; set; }
}