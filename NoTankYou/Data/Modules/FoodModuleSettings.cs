using NoTankYou.Data.Components;

namespace NoTankYou.Data.Modules
{
    public class FoodModuleSettings : GenericSettings
    {
        public new int Priority = 2;
        public int FoodEarlyWarningTime = 600;
        public bool SavageDuties = false;
        public bool UltimateDuties = false;
    }
}
