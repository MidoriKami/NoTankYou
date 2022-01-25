namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class BlueMageSettings : BannerSettings
    {
        public BlueMageSettings()
        {
            CategoryString = "Blue Mage Settings";
        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.BlueMageSettings;
        public override void Dispose()
        {
        }
    }
}
