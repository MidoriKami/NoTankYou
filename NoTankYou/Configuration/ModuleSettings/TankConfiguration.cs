using NoTankYou.Configuration.Components;

namespace NoTankYou.Configuration.ModuleSettings;

public class TankConfiguration : GenericSettings
{
    public Setting<bool> DisableInAllianceRaid = new(true);
    public Setting<bool> CheckAllianceStances = new(false);
}