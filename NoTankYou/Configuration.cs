using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        public bool DisableInAllianceRaid = true;
        public bool ShowMoveWarningBanner = true;
        public int InstanceLoadDelayTime = 8000;
        public List<int> TerritoryBlacklist;

        public void PrintBlacklist()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            Service.Chat.Print("[NoTankYou] Blacklist: {" + string.Join(", ", TerritoryBlacklist) + "}");
            Service.Chat.Print($"[NoTankYou] Current Territory: {currentTerritory}");
            
        }

        public void AddCurrentTerritoryToBlacklist()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            if(TerritoryBlacklist == null)
            {
                TerritoryBlacklist = new List<int>();
            }

            if(!TerritoryBlacklist.Contains(currentTerritory))
            {
                TerritoryBlacklist.Add(currentTerritory);
                Service.Chat.Print($"[NoTankYou] Added {currentTerritory} to blacklist");
            }
        }

        public void RemoveCurrentTerritoryFromBlacklist()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            if(TerritoryBlacklist.Contains(currentTerritory))
            {
                TerritoryBlacklist.Remove(currentTerritory);
                Service.Chat.Print($"[NoTankYou] Removed {currentTerritory} from blacklist");
            }
        }

        public void PrintStatus()
        {
            var chat = Service.Chat;

            chat.Print($"[NoTankYou] Disabled In Alliance Raid: {DisableInAllianceRaid}");
            chat.Print($"[NoTankYou] Enable BannerMovement: {ShowMoveWarningBanner}");
            chat.Print($"[NoTankYou] Instance Load Delay Time: {InstanceLoadDelayTime}");
        }

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