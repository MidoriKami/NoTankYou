namespace NoTankYou.Configuration.Components;

public class WarningState
{
    public string IconLabel { get; init; } = string.Empty;
    public uint IconID { get; init; }
    public string MessageLong { get; init; } = string.Empty;
    public string MessageShort { get; init; } = string.Empty;
    public int Priority { get; init; }
}