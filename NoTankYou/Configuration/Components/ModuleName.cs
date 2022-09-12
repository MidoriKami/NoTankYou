using System;
using NoTankYou.Localization;

namespace NoTankYou.Configuration.Components;

public enum ModuleName
{
    Tanks,
    BlueMage,
    Dancer,
    Food,
    FreeCompany,
    Sage,
    Scholar,
    Summoner,
    Spiritbond,
}

public static class ModuleNameExtensions
{
    public static string GetTranslatedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.Tanks => Strings.Modules.Tank.Label,
            ModuleName.BlueMage => Strings.Modules.BlueMage.Label,
            ModuleName.Dancer => Strings.Modules.Dancer.Label,
            ModuleName.Food => Strings.Modules.Food.Label,
            ModuleName.FreeCompany => Strings.Modules.FreeCompany.Label,
            ModuleName.Sage => Strings.Modules.Sage.Label,
            ModuleName.Scholar => Strings.Modules.Scholar.Label,
            ModuleName.Summoner => Strings.Modules.Summoner.Label,
            ModuleName.Spiritbond => Strings.Modules.SpiritBond.Label,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}