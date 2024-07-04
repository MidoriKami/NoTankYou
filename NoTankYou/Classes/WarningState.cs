namespace NoTankYou.Classes;

public class WarningState {
    public required uint IconId { get; init; }
    public required string IconLabel { get; init; }
    public required string Message { get; init; } = string.Empty;
    public required int Priority { get; init; }
    public required string SourcePlayerName { get; init; } = string.Empty;
    public required ulong SourceEntityId { get; init; }
    public required ModuleName SourceModule { get; init; }
}