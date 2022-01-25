using NoTankYou.SettingsSystem.DisplayModules.SettingsTab;

namespace NoTankYou.SettingsSystem.SettingsTabs
{
    internal class SettingsTab : TabCategory
    {
        public SettingsTab()
        {
            CategoryName = "Settings";
            TabName = "Settings";

            Modules = new()
            {
                new DisplayModules.SettingsTab.BannerSettings(),
                new GeneralSettings(),
                new BlacklistSettings()
            };
        }
    }
}
