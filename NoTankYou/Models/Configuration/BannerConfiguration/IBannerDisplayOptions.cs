using KamiLib.AutomaticUserInterface;
using NoTankYou.DataModels;

namespace NoTankYou.Models.BannerConfiguration;

[Category("DisplayOptions", 2)]
public interface IBannerDisplayOptions
{
    [EnumConfig("DisplayMode", "DisplayModeHelp")]
    public BannerOverlayDisplayMode DisplayMode { get; set; }
    
    [IntConfig("MaxWarnings", 1, 10)]
    public int WarningCount { get; set; }

    [FloatConfig("AdditionalSpacing", 0.0f, 100.0f)]
    public float AdditionalSpacing { get; set; }
}