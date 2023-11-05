using KamiLib.AutomaticUserInterface;
using NoTankYou.Abstracts;

namespace NoTankYou.Models.ModuleConfiguration;

[Category("ModuleOptions")]
public class TankConfiguration : IModuleConfigBase
{
    public bool Enabled { get; set; } = false;
    public bool SoloMode { get; set; } = false;
    public bool DutiesOnly { get; set; } = false;
    public bool DisableInSanctuary { get; set; } = false;
    public bool DisableWhileRolePlaying { get; set; } = true;
    public int Priority { get; set; } = 4;
    public bool CustomWarning { get; set; } = false;
    public string CustomWarningText { get; set; } = string.Empty;
    
    [BoolConfig("DisableInAllianceRaid", "DisableInAllianceRaidHelp")]
    public bool DisableInAlliance { get; set; } = true;

    [BoolConfig("CheckAllianceTanks", "CheckAllianceTanksHelp")]
    public bool CheckAllianceTanks { get; set; } = true;
}