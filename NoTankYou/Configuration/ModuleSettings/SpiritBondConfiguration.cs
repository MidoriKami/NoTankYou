using NoTankYou.Configuration.Components;

namespace NoTankYou.Configuration.ModuleSettings;

public class SpiritBondConfiguration : GenericSettings 
{
    public Setting<int> SpiritBondEarlyWarningTime = new(600);
    public Setting<bool> SavageDuties = new(false);
    public Setting<bool> UltimateDuties = new(false);
    public Setting<bool> ExtremeUnreal = new(false);
    public Setting<bool> DisableInCombat = new(false);
}