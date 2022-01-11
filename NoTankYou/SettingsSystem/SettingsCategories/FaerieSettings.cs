using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class FaerieSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.FaerieSettings;

        public FaerieSettings()
        {
            CategoryName = "Scholar Faerie Settings";
            TabName = "SCH";
        }
    }
}
