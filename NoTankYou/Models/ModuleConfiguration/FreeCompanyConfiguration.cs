using KamiLib.AutomaticUserInterface;
using NoTankYou.Abstracts;
using NoTankYou.Models.Attributes;
using NoTankYou.Models.Enums;

namespace NoTankYou.Models.ModuleConfiguration;

[Category("ModuleOptions")]
public class FreeCompanyConfiguration : IModuleConfigBase
{
    [Disabled] public bool SoloMode { get; set; }
    [Disabled] public bool DutiesOnly { get; set; }
    [Disabled] public bool DisableInSanctuary { get; set; }
    [Disabled] public bool DisableWhileRolePlaying { get; set; } = false;

    public bool Enabled { get; set; } = false;
    public int Priority { get; set; } = 2;
    public bool CustomWarning { get; set; } = false;
    public string CustomWarningText { get; set; } = string.Empty;
    

    [EnumConfig("ModeSelect", "FreeCompanyModeHelp")]
    public FreeCompanyMode Mode = FreeCompanyMode.Any;

    [IntComboConfig("BuffCount", 1, 2, "FreeCompanyBuffCountHelp")]
    public int BuffCount { get; set; } = 2;
    
    [FreeCompanyStatusSelector("FirstBuff")]
    public uint PrimaryBuff { get; set; } = 360;

    [FreeCompanyStatusSelector("SecondBuff")]
    public uint SecondaryBuff { get; set; } = 364;
}