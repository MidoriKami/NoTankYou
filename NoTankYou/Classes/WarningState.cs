using System;

namespace NoTankYou.Classes;

public class WarningState {
    public required uint IconId { get; init; }
    public required uint ActionId { get; init; }
    public required string IconLabel { get; init; }
    public required string Message { get; init; } = string.Empty;
    public required int Priority { get; init; }
    public required string SourcePlayerName { get; init; } = string.Empty;
    public required ulong SourceEntityId { get; init; }
    public required ModuleName SourceModule { get; init; }

    public static bool operator ==(WarningState? left, WarningState? right) {
        if (left is null) return false;
        if (right is null) return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(WarningState? left, WarningState? right)
        => !(left == right);

    private bool Equals(WarningState other)
        => SourceEntityId == other.SourceEntityId && SourceModule == other.SourceModule;

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((WarningState) obj);
    }

    public override int GetHashCode()
        => HashCode.Combine(SourceEntityId, (int) SourceModule);
}