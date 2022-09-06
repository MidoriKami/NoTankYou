using NoTankYou.Configuration.Components;

namespace NoTankYou.Configuration.ModuleSettings;

public class FoodConfiguration : GenericSettings
{
    public Setting<int> FoodEarlyWarningTime = new(600);
    public Setting<bool> SavageDuties = new(false);
    public Setting<bool> UltimateDuties = new(false);
    public Setting<bool> ExtremeUnreal = new(false);
    public Setting<bool> DisableInCombat = new(true);
}