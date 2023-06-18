using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Abstracts;

public class ModuleConfigBase
{
    [BoolConfigOption("Enable", "Options", -1)]
    public bool Enabled = false;

    [BoolConfigOption("SoloMode", "Options", -1, "SoloModeHelp")]
    public bool SoloMode = false;
    
    [BoolConfigOption("DutiesOnly", "Options", -1, "DutiesOnlyHelp")]
    public bool DutiesOnly = false;
        
    [BoolConfigOption("HideInSanctuary", "Options", -1, "HideInSanctuaryHelp")]
    public bool DisableInSanctuary = false;
    
    [IntCounterConfigOption("Priority", "Options", -1, "PriorityHelp")]
    public int Priority = 1;

    [BoolConfigOption("CustomWarning", "DisplayOptions", 0)]
    public bool CustomWarning = false;

    [StringConfigOption("CustomWarningText", "DisplayOptions", 0, true)]
    public string CustomWarningText = string.Empty;
}