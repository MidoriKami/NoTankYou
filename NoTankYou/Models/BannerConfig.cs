using System.Numerics;
using KamiLib.AutomaticUserInterface;
using NoTankYou.DataModels;

namespace NoTankYou.Models;

public class BannerConfig
{
    [BoolConfigOption("Enable", "DisplayOptions", 0)]
    public bool Enabled = true;

    [BoolConfigOption("SoloMode", "DisplayOptions", 0, "SoloModeHelp")]
    public bool SoloMode = false;

    [BoolConfigOption("SampleMode", "DisplayOptions", 0)]
    public bool SampleMode = false;
    
    [BoolConfigOption("WindowDragging", "Positioning", 1)]
    public bool CanDrag = true;
    
    [PositionConfigOption("Position", "Positioning", 1)]
    public Vector2 WindowPosition = new(700.0f, 400.0f);

    [FloatConfigOption("Scale", "Positioning", 1, 0.25f, 3.0f)]
    public float Scale = 1.0f;

    [EnumConfigOption("DisplayMode", "DisplayOptions", 2, "DisplayModeHelp")]
    public BannerOverlayDisplayMode DisplayMode = BannerOverlayDisplayMode.List;
    
    [IntConfigOption("MaxWarnings", "DisplayOptions", 2, 1, 10)]
    public int WarningCount = 10;

    [FloatConfigOption("AdditionalSpacing", "DisplayOptions", 2, 0.0f, 100.0f)]
    public float AdditionalSpacing = 0.0f;
    
    [BoolConfigOption("WarningShield", "DisplayStyle", 3)]
    public bool WarningShield = true;

    [BoolConfigOption("WarningText", "DisplayStyle", 3)]
    public bool WarningText = true; 

    [BoolConfigOption("PlayerNames", "DisplayStyle", 3)]
    public bool PlayerNames = true;
    
    [BoolConfigOption("ActionName", "DisplayStyle", 3)]
    public bool ShowActionName = true;

    [BoolConfigOption("ActionIcon", "DisplayStyle", 3)]
    public bool Icon = true;
}