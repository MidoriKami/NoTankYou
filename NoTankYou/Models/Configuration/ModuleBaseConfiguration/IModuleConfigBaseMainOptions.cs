using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Abstracts;

[Category("Options", -2)]
public interface IModuleConfigBaseMainOptions
{
    [BoolConfig("Enable")]
    public bool Enabled { get; set; }

    [BoolConfig("SoloMode","SoloModeHelp")]
    public bool SoloMode { get; set; }
    
    [BoolConfig("DutiesOnly", "DutiesOnlyHelp")]
    public bool DutiesOnly { get; set; }
        
    [BoolConfig("HideInSanctuary", "HideInSanctuaryHelp")]
    public bool DisableInSanctuary { get; set; }

    [BoolConfig("DisableWhileRolePlaying", "DisableWhileRolePlayingHelp")]
    public bool DisableWhileRolePlaying { get; set; }
    
    [IntCounterConfig("Priority", "PriorityHelp")]
    public int Priority { get; set; }
}