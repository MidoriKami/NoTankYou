using System.Drawing;
using System.Numerics;
using KamiLib.AutomaticUserInterface;
using KamiLib.Utilities;

namespace NoTankYou.Models;

public class PartyListConfig
{
    [BoolConfigOption("Enable", "DisplayOptions", 0)]
    public bool Enabled = true;
    
    [BoolConfigOption("SoloMode", "DisplayOptions", 0, "SoloModeHelp")]
    public bool SoloMode = false;

    [BoolConfigOption("SampleMode", "DisplayOptions", 0)]
    public bool SampleMode = true;

    [BoolConfigOption("WarningText", "DisplayStyle", 1)]
    public bool WarningText = true;

    [BoolConfigOption("PlayerNames", "DisplayStyle", 1)]
    public bool PlayerName = true;

    [BoolConfigOption("JobIcon", "DisplayStyle", 1)]
    public bool JobIcon = true;

    [BoolConfigOption("Animation", "DisplayStyle", 1)]
    public bool Animation = true;

    [FloatConfigOption("AnimationPeriod", "DisplayStyle", 1, 500, 5000)]
    public float AnimationPeriod = 1000;

    [ColorConfigOption("TextColor", "DisplayColors", 2, 1.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TextColor = KnownColor.Red.AsVector4();

    [ColorConfigOption("OutlineColor", "DisplayColors", 2, 1.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 OutlineColor = KnownColor.Red.AsVector4();
}