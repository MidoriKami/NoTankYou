using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models;

[Category("DisplayOptions")]
public interface IPartyListMainOptions
{
    [BoolConfig("Enable")]
    public bool Enabled { get; set; }
    
    [BoolConfig("SoloMode", "SoloModeHelp")]
    public bool SoloMode { get; set; }

    [BoolConfig("SampleMode")]
    public bool SampleMode { get; set; }
}