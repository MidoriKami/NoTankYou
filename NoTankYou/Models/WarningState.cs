namespace NoTankYou.DataModels;

public class WarningState
{
    public required uint IconId { get; init; }
    public required string IconLabel { get; init; }
    public required string Message { get; init; } = string.Empty;
    public required int Priority { get; init; }
    public required string SourcePlayerName { get; init; } = string.Empty;
    public required ulong SourceObjectId { get; init; }
}