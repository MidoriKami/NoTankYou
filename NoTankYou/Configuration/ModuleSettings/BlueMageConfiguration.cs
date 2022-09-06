using NoTankYou.Configuration.Components;

namespace NoTankYou.Configuration.ModuleSettings;

public class BlueMageConfiguration : GenericSettings
{
    public Setting<bool> Mimicry = new(false);
    public Setting<bool> TankStance = new(false);
    public Setting<bool> BasicInstinct = new(false);
}