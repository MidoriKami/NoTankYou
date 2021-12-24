using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class KardionSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.KardionSettings;


        public KardionSettings() : base("Kardion Settings")
        {
        }
    }
}
