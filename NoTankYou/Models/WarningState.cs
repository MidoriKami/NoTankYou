namespace NoTankYou.DataModels;

public class WarningState
{
    public required uint ActionId { get; init; }
    public required string Message { get; init; } = string.Empty;
    public required string MessageLong { get; init; } = string.Empty;
    public required int Priority { get; init; }
    public required string SourcePlayerName { get; init; } = string.Empty;
    public required ulong SourceObjectId { get; init; }
}