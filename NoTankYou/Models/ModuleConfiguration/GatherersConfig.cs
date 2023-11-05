using KamiLib.AutomaticUserInterface;
using NoTankYou.Abstracts;

namespace NoTankYou.Models.ModuleConfiguration;

[Category("JobWarnings")]
public class GatherersConfig : IModuleConfigBase
{
    public bool Enabled { get; set; } = false;
    [Disabled] public bool SoloMode { get; set; } = true;
    [Disabled] public bool DutiesOnly { get; set; } = false;
    public bool DisableInSanctuary { get; set; }
    public int Priority { get; set; } = 3;
    public bool CustomWarning { get; set; }
    public string CustomWarningText { get; set; } = string.Empty;

    [BoolConfig("Miner")]
    public bool Miner { get; set; } = true;
    
    [BoolConfig("Botanist")]
    public bool Botanist { get; set; } = true;
    
    [BoolConfig("Fisher")]
    public bool Fisher { get; set; } = true;

    [BoolConfig("CollectorsGlove")] 
    public bool CollectorsGlove = true;
}