using KamiLib.AutomaticUserInterface;
using NoTankYou.Abstracts;

namespace NoTankYou.Models.ModuleConfiguration;

[Category("ModuleOptions")]
public class ChocoboConfiguration : IModuleConfigBase
{
    [Disabled] public bool SoloMode { get; set; } = false;
    [Disabled] public bool DutiesOnly { get; set; } = false;
    [Disabled] public bool DisableInSanctuary { get; set; } = false;
    [Disabled] public bool DisableWhileRolePlaying { get; set; } = false;
    
    public bool Enabled { get; set; } = false;
    public int Priority { get; set; } = 1;
    public bool CustomWarning { get; set; } = false;
    public string CustomWarningText { get; set; } = string.Empty;

    [BoolConfig("SuppressInCombat")]
    public bool DisableInCombat { get; set; } = true;

    [BoolConfig("EarlyWarning")]
    public bool EarlyWarning { get; set; } = true;
    
    [IntCounterConfig("EarlyWarningTime", false)]
    public int EarlyWarningTime { get; set; } = 300;
}
