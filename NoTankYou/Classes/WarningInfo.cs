using FFXIVClientStructs.FFXIV.Client.Game.Character;
using NoTankYou.Enums;

namespace NoTankYou.Classes;

public unsafe class WarningInfo {
    public int Priority;

    public uint IconId;
    public uint ActionId;

    public BattleChara* SourceCharacter;

    public string SourceModule = string.Empty;
    public string IconLabel = string.Empty;
    public string Message = string.Empty;
}
