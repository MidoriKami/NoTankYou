namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class TankStanceSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.TankStanceSettings;

        public TankStanceSettings()
        {
            CategoryString = "Tank Stance Settings";
        }

        public override void Dispose()
        {
            
        }
    }
}
