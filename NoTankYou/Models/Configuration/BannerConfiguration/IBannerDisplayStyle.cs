using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models.BannerConfiguration;

[Category("DisplayStyle", 3)]
public interface IBannerDisplayStyle
{
    [BoolConfig("WarningShield")]
    public bool WarningShield { get; set; }

    [BoolConfig("WarningText")]
    public bool WarningText { get; set; } 

    [BoolConfig("PlayerNames")]
    public bool PlayerNames { get; set; }
    
    [BoolConfig("ActionName")]
    public bool ShowActionName { get; set; }

    [BoolConfig("ActionIcon")]
    public bool Icon { get; set; }
}