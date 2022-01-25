namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class DancePartnerSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.DancePartnerSettings;

        public DancePartnerSettings()
        {
            CategoryString = "Dance Partner Settings";
        }

        public override void Dispose()
        {
        }
    }
}
