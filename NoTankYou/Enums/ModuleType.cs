using System.ComponentModel;

namespace NoTankYou.Enums;

public enum ModuleType {

    [Description("General Features")]
    GeneralFeatures,
    
    [Description("Class Features")]
	ClassFeatures,
    
    Hidden,
}
