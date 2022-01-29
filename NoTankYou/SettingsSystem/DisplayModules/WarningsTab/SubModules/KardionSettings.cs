namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class KardionSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.KardionSettings;


        public KardionSettings()
        {
            CategoryString = "Sage Kardion";
        }

        public override void Dispose()
        {
            
        }
    }
}
