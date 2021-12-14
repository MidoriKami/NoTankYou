using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 3;

        public bool DiableInAllianceRaid = true;
        public int TerritoryChangeDelayTime = 8000;

        public bool AllEnabled = false;
        public bool EnableTankStanceBanner = false;

        public bool ForceShowTankStanceBanner = false;

        public bool RepositionModeTankStanceBanner = false;

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