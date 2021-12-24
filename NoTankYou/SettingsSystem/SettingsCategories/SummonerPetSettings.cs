using ImGuiNET;

namespace NoTankYou.SettingsSystem.SettingsCategories
{
    internal class SummonerPetSettings : BannerSettings
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.SummonerSettings;


        public SummonerPetSettings() : base("Summoner Pet Settings")
        {
        }
    }
}
