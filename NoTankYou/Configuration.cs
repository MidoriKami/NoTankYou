using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using NoTankYou.DisplaySystem;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 3;
        public int NumberOfWaitFrames = 30;

        public WarningBanner.ImageSize ImageSize = WarningBanner.ImageSize.Large;

        public enum MainMode
        {
            Party,
            Solo
        }

        public enum SubMode
        {
            OnlyInDuty,
            Everywhere
        }

        public MainMode ProcessingMainMode = MainMode.Party;
        public SubMode ProcessingSubMode = SubMode.OnlyInDuty;

        public bool DisableInAllianceRaid = true;
        public int TerritoryChangeDelayTime = 8000;
        public int DeathGracePeriod = 5000;

        public bool EnableTankStanceBanner = false;
        public bool EnableDancePartnerBanner = false;
        public bool EnableFaerieBanner = false;
        public bool EnableKardionBanner = false;
        public bool EnableSummonerBanner = false;

        public bool ForceShowTankStanceBanner = false;
        public bool ForceShowDancePartnerBanner = false;
        public bool ForceShowFaerieBanner = false;
        public bool ForceShowKardionBanner = false;
        public bool ForceShowSummonerBanner = false;

        public bool RepositionModeTankStanceBanner = false;
        public bool RepositionModeDancePartnerBanner = false;
        public bool RepositionModeFaerieBanner = false;
        public bool RepositionModeKardionBanner = false;
        public bool RepositionModeSummonerBanner = false;

        public List<int> TerritoryBlacklist = new();

        public bool ForceWindowUpdate = false;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}