using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models;

[Category("DisplayStyle", 1)]
public interface IPartyListDisplayStyle
{
    [BoolConfig("WarningText")]
    public bool WarningText { get; set; }

    [BoolConfig("PlayerNames")]
    public bool PlayerName { get; set; }

    [BoolConfig("JobIcon")]
    public bool JobIcon { get; set; }

    [BoolConfig("Animation")]
    public bool Animation { get; set; }

    [FloatConfig("AnimationPeriod", 500, 5000)]
    public float AnimationPeriod { get; set; }
}