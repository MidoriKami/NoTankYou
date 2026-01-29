using System.Collections.Generic;

namespace NoTankYou.Classes;

public class ConfigBase : Savable {
    public bool WaitForDutyStart = true;
    public bool SoloMode = false;
    public bool DutiesOnly = true;
    public bool DisableInSanctuary = true;
    public bool PartyMembersOnly = true;
    public bool AutoSuppress = false;
    public int AutoSuppressTime = 30;
    public int Priority = 0;
    public string CustomWarningText = string.Empty;

    public List<uint> BlacklistedZones = [];
    
    protected override string FileExtension => ".config.json";
}
