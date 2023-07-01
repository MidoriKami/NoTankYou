using KamiLib.AutomaticUserInterface;
using NoTankYou.Abstracts;

namespace NoTankYou.Models.ModuleConfiguration;

[Category("ZoneFilter", 1)]
public interface IZoneFilter
{
    [BoolConfig("Savage")]
    public bool SavageFilter { get; set; }

    [BoolConfig("Ultimate")]
    public bool UltimateFilter { get; set; }

    [BoolConfig("ExtremeUnreal")]
    public bool ExtremeUnrealFilter { get; set; }

    [BoolConfig("Criterion")]
    public bool CriterionFilter { get; set; }
}

[Category("ModuleOptions")]
public class ConsumableConfiguration : IModuleConfigBase, IZoneFilter
{
    public bool Enabled { get; set; } = false;
    public bool SoloMode { get; set; } = false;
    public bool DutiesOnly { get; set; } = false;
    public bool DisableInSanctuary { get; set; } = false;
    public int Priority { get; set; } = 3;
    public bool CustomWarning { get; set; } = false;
    public string CustomWarningText { get; set; } = string.Empty;
    
    [BoolConfig("SuppressInCombat")]
    public bool SuppressInCombat { get; set; } = true;

    [IntCounterConfig("EarlyWarningTime", false)]
    public int EarlyWarningTime { get; set; } = 600;
    
    // IZoneFilter
    public bool ZoneFilter { get; set; } = false;
    public bool SavageFilter { get; set; } = false;
    public bool UltimateFilter { get; set; } = false;
    public bool ExtremeUnrealFilter { get; set; } = false;
    public bool CriterionFilter { get; set; } = false;
}