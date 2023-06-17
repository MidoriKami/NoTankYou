using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

public class SystemConfig
{
    public string CharacterName { get; set; } = string.Empty;
    public string CharacterWorld { get; set; } = string.Empty;

    [BoolConfigOption("WaitForDutyStart", "GeneralOptions", 0, "WaitForDutyStartHelp")]
    public bool WaitUntilDutyStart = true;
}