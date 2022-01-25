namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class FaerieSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.FaerieSettings;

        public FaerieSettings()
        {
            CategoryString = "Scholar Faerie Settings";
        }

        public override void Dispose()
        {
        }
    }
}
