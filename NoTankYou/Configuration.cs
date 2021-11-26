using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool DisableInAllianceRaid = true;
        public bool ShowNoTankWarning = false;
        public bool ForceShowNoTankWarning = false;
        public bool EnableClickthrough = true;
        public int InstanceLoadDelayTime = 8000;
        public bool PluginPaused = false;
        public List<int> TerritoryBlacklist;

        public void PrintBlacklist()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            Service.Chat.Print("Blacklist: {" + string.Join(", ", TerritoryBlacklist) + "}");
            Service.Chat.Print($"Current Territory: {currentTerritory}");
            
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
                Service.Chat.Print($"Added {currentTerritory} to blacklist");
            }
        }

        public void RemoveCurrentTerritoryFromBlacklist()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            if(TerritoryBlacklist.Contains(currentTerritory))
            {
                TerritoryBlacklist.Remove(currentTerritory);
                Service.Chat.Print($"Removed {currentTerritory} from blacklist");
            }
        }

        public void PrintStatus()
        {
            var chat = Service.Chat;

            chat.Print($"Version: {Version}");
            chat.Print($"Disabled In Alliance Raid: {DisableInAllianceRaid}");
            chat.Print($"Show No Tank Warning: {ShowNoTankWarning}");
            chat.Print($"Force Show No Tank Warning: {ForceShowNoTankWarning}");
            chat.Print($"Enable Clickthrough: {EnableClickthrough}");
            chat.Print($"Instance Load Delay Time: {InstanceLoadDelayTime}");
            chat.Print($"Paused: {PluginPaused}");
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