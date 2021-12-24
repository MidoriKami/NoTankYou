using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class DancePartnerSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.DancePartnerSettings;

        public DancePartnerSettings() : base("Dance Partner Settings")
        {
        }
    }
}
