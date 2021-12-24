using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class TankStanceSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.TankStanceSettings;

        public TankStanceSettings() : base("Tank Stance Settings")
        {
        }
    }
}
