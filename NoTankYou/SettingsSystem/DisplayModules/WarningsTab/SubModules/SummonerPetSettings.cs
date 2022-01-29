namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class SummonerPetSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.SummonerSettings;


        public SummonerPetSettings()
        {
            CategoryString = "Summoner Pet";
        }

        public override void Dispose()
        {
        }
    }
}
