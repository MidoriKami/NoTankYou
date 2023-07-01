using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models.Enums;

public enum AllianceMode
{
    [EnumLabel("DisableInAllianceRaid")]
    Disable,
    
    [EnumLabel("IncludeAlliance")]
    Alliance,
}