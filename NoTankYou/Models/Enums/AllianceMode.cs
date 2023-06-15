using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models.Enums;

public enum AllianceMode
{
    [EnumLabel("DisableInAllianceRaid")]
    Disable,
    
    [EnumLabel("PartyOnly")]
    PartyOnly,
    
    [EnumLabel("IncludeAlliance")]
    Alliance,
}