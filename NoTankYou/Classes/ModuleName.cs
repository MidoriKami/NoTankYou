using System;
using System.ComponentModel;

namespace NoTankYou.Classes;

[AttributeUsage(AttributeTargets.Field)]
public class ModuleIconAttribute(uint moduleIcon) : Attribute {
    public uint ModuleIcon { get; } = moduleIcon;
}

public enum ModuleName {
    [Description("Tank")]
    [ModuleIcon(62019)]
    Tanks,
    
    [Description("Blue Mage")]
    [ModuleIcon(62036)]
    BlueMage,
    
    [Description("Dancer")]
    [ModuleIcon(62038)]
    Dancer,
    
    [Description("Free Company")]
    [ModuleIcon(60460)]
    FreeCompany,
    
    [Description("Food")]
    [ModuleIcon(62015)]
    Food,
    
    [Description("Spiritbond")]
    [ModuleIcon(62014)]
    SpiritBond,
    
    [Description("Sage")]
    [ModuleIcon(62040)]
    Sage,
    
    [Description("Scholar")]
    [ModuleIcon(62028)]
    Scholar,
    
    [Description("Summoner")]
    [ModuleIcon(62027)]
    Summoner,
    
    [Description("Chocobo")]
    [ModuleIcon(62043)]
    Chocobo,
        
    [Description("Gatherers")]
    [ModuleIcon(62017)]
    Gatherers,
    
    [Description("Monk")]
    [ModuleIcon(62020)]
    Monk,
    
    [Description("Pictomancer")]
    [ModuleIcon(62042)]
    Pictomancer,
    
    [Description("Reaper")]
    [ModuleIcon(62039)]
    Reaper,
    
    [Description("Test")]
    [ModuleIcon(62144)]
    Test,
}