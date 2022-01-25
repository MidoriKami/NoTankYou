using NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules;

namespace NoTankYou.SettingsSystem.SettingsTabs
{
    internal class WarningsTab : TabCategory
    {

        public WarningsTab()
        {
            CategoryName = "Warning Banner Settings";
            TabName = "Warnings";

            Modules = new()
            {
                new TankStancesSettingsModule(),
                new DancePartnerSettings(),
                new FaerieSettings(),
                new KardionSettings(),
                new SummonerPetSettings(),
                new FoodSettings()
            };
        }

        protected override void DrawContents()
        {
            
        }
    }
}
