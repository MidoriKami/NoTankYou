using System;
using System.ComponentModel;

namespace NoTankYou.Classes;

public class ModuleIconAttribute(uint moduleIcon, uint simpleIcon, uint backgroundIcon = 0) : Attribute {
    public uint ModuleIcon { get; } = moduleIcon;
    public uint BackgroundIcon { get; } = backgroundIcon;
    public uint SimpleIcon { get; } = simpleIcon;
}

public enum ModuleName {
    [Description("Tank")]
    [ModuleIcon(62581, 62019)]
    Tanks,
    
    [Description("Blue Mage")]
    [ModuleIcon(62136, 62036)]
    BlueMage,
    
    [Description("Dancer")]
    [ModuleIcon(62138, 62038)]
    Dancer,
    
    [Description("Free Company")]
    [ModuleIcon(60460, 60460, 62574)]
    FreeCompany,
    
    [Description("Food")]
    [ModuleIcon(62015, 62015, 62574)]
    Food,
    
    [Description("Spiritbond")]
    [ModuleIcon(62014, 62014, 62574)]
    SpiritBond,
    
    [Description("Sage")]
    [ModuleIcon(62140, 62040)]
    Sage,
    
    [Description("Scholar")]
    [ModuleIcon(62128, 62028)]
    Scholar,
    
    [Description("Summoner")]
    [ModuleIcon(62127, 62027)]
    Summoner,
    
    [Description("Chocobo")]
    [ModuleIcon(62143, 62043)]
    Chocobo,
        
    [Description("Gatherers")]
    [ModuleIcon(62203, 62203, 62574)]
    Gatherers,
    
    [Description("Test")]
    [ModuleIcon(62144, 62144)]
    Test,
}