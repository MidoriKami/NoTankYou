using System.ComponentModel;

namespace NoTankYou.Enums;

public enum ModuleType {
    [Description("Warning Displays")]
    WarningDisplays,
    
    [Description("Class Features")]
	ClassFeatures,
    
    [Description("Other Features")]
    OtherFeatures,

    Hidden,
}
