using System;
using System.ComponentModel;

namespace NoTankYou.Classes;

public class ModuleIconAttribute(uint moduleIcon, uint backgroundIcon = 0) : Attribute {
    public uint ModuleIcon { get; } = moduleIcon;
    public uint BackgroundIcon { get; } = backgroundIcon;
}

public enum ModuleName {
    [Description("Tank")]
    [ModuleIcon(62581)]
    Tanks,
    
    [Description("Blue Mage")]
    [ModuleIcon(62136)]
    BlueMage,
    
    [Description("Dancer")]
    [ModuleIcon(62138)]
    Dancer,
    
    [Description("Free Company")]
    [ModuleIcon(60460, 62574)]
    FreeCompany,
    
    [Description("Food")]
    [ModuleIcon(62015, 62574)]
    Food,
    
    [Description("Spiritbond")]
    [ModuleIcon(62014, 62574)]
    SpiritBond,
    
    [Description("Sage")]
    [ModuleIcon(62140)]
    Sage,
    
    [Description("Scholar")]
    [ModuleIcon(62128)]
    Scholar,
    
    [Description("Summoner")]
    [ModuleIcon(62127)]
    Summoner,
    
    [Description("Chocobo")]
    [ModuleIcon(62141)]
    Chocobo,
        
    [Description("Gatherers")]
    [ModuleIcon(62203, 62574)]
    Gatherers,
    
    [Description("Test")]
    [ModuleIcon(62144)]
    Test,
}