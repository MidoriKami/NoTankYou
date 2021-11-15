using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace NoTankYou
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool ShowNoTankWarning = false;
        public bool ForceShowNoTankWarning = false;
        public bool DisableClickthrough = false;
        public int InstanceLoadDelayTime = 5000;
        public bool DisableInAllianceRaids = true;

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}