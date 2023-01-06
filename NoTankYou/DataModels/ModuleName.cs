using System;
using NoTankYou.Localization;

namespace NoTankYou.DataModels;

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
    Cutscene,
}

public static class ModuleNameExtensions
{
    public static string GetTranslatedString(this ModuleName value)
    {
        return value switch
        {
            ModuleName.Tanks => Strings.Tank_Label,
            ModuleName.BlueMage => Strings.BlueMage_Label,
            ModuleName.Dancer => Strings.Dancer_Label,
            ModuleName.Food => Strings.Food_Label,
            ModuleName.FreeCompany => Strings.FreeCompany_Label,
            ModuleName.Sage => Strings.Sage_Label,
            ModuleName.Scholar => Strings.Scholar_Label,
            ModuleName.Summoner => Strings.Summoner_Label,
            ModuleName.Spiritbond => Strings.SpiritBond_Label,
            ModuleName.Cutscene => Strings.Cutscene_Label,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}