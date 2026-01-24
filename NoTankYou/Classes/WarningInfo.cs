using System;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace NoTankYou.Classes;

public unsafe class WarningInfo {
    public int Priority;

    public uint IconId;
    public uint ActionId;
    public uint ModuleIcon;

    public BattleChara* SourceCharacter {
        get;
        init {
            field = value;
            if (value is null) throw new Exception($"Source Character for warning {SourceModule} was set to null");
        }
    }

    public string SourceModule = string.Empty;
    public string IconLabel = string.Empty;
    public string Message = string.Empty;
}
