using System;
using Dalamud.Configuration;

namespace NoTankYou.Data
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool DeveloperMode = false;

        public void Save() => Service.PluginInterface.SavePluginConfig(this);
    }
}
