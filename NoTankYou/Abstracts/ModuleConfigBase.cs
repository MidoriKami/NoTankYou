using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Abstracts;

public class ModuleConfigBase
{
    [BoolConfigOption("Labels_Enabled", "Labels_Options", -1)]
    public bool Enabled = false;

    [BoolConfigOption("BannerOverlay_SoloMode", "Labels_Options", -1)]
    public bool SoloMode = false;
    
    [BoolConfigOption("Configuration_DutiesOnly", "Labels_Options", -1)]
    public bool DutiesOnly = false;
        
    [BoolConfigOption("Configuration_HideInSanctuary", "Labels_Options", -1)]
    public bool DisableInSanctuary = true;
    
    [IntCounterConfigOption("Labels_Priority", "Labels_Options", -1)]
    public int Priority = 1;

    [BoolConfigOption("Configuration_CustomLongWarningEnable", "Warning_Options", 0)]
    public bool UseCustomLongWarning = false;

    [StringConfigOption("Configuration_CustomLongWarningText", "Warning_Options", 0, true)]
    public string CustomLongWarningText = string.Empty;
    
    [BoolConfigOption("Configuration_CustomShortWarningEnable", "Warning_Options", 0)]
    public bool UseCustomShortWarning = false;

    [StringConfigOption("Configuration_CustomShortWarningText", "Warning_Options", 0, true)]
    public string CustomShortWarningText = string.Empty;
}