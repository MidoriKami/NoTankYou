using System.Numerics;
using KamiLib.AutomaticUserInterface;
using NoTankYou.DataModels;

namespace NoTankYou.Models;

public class BannerConfig
{
    [BoolConfigOption("Enabled", "DisplayOptions", 0)]
    public bool Enabled = true;

    [BoolConfigOption("SampleMode", "DisplayOptions", 0)]
    public bool SampleMode = true;
    
    [BoolConfigOption("SoloMode", "DisplayOptions", 0)]
    public bool SoloMode = false;
    
    [BoolConfigOption("EnableWindowDragging", "PositionOptions", 1)]
    public bool CanDrag = true;
    
    [PositionConfigOption("Position", "PositionOptions", 1)]
    public Vector2 WindowPosition = new(700.0f, 400.0f);

    [FloatConfigOption("Scale", "PositionOptions", 1, 0.25f, 3.0f)]
    public float Scale = 1.0f;

    [EnumConfigOption("DisplayMode", "DisplayOptions", 2)]
    public BannerOverlayDisplayMode DisplayMode = BannerOverlayDisplayMode.List;
    
    [IntConfigOption("MaxDisplayedWarningCount", "DisplayOptions", 2, 1, 10)]
    public int WarningCount = 10;
    
    [BoolConfigOption("WarningShield", "DisplayStyle", 3)]
    public bool WarningShield = true;

    [BoolConfigOption("WarningText", "DisplayStyle", 3)]
    public bool WarningText = true; 

    [BoolConfigOption("PlayerNames", "DisplayStyle", 3)]
    public bool PlayerNames = true;
    
    [BoolConfigOption("ActionName", "DisplayStyle", 3)]
    public bool ShowActionName = true;

    [BoolConfigOption("Icon", "DisplayStyle", 3)]
    public bool Icon = true;
}