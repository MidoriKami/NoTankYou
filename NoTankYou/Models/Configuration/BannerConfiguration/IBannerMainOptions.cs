using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models.BannerConfiguration;

[Category("DisplayOptions")]
public interface IBannerMainOptions
{
    [BoolConfig("Enable")]
    public bool Enabled { get; set; }

    [BoolConfig("SoloMode", "SoloModeHelp")]
    public bool SoloMode { get; set; }

    [BoolConfig("SampleMode")]
    public bool SampleMode { get; set; }
}