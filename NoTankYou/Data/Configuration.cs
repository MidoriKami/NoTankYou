using System;
using Dalamud.Configuration;

namespace NoTankYou.Data
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool DeveloperMode = false;

        public ModuleSettings ModuleSettings { get; set; } = new();
        public DisplaySettings DisplaySettings { get; set; } = new();
        public SystemSettings SystemSettings { get; set; } = new();

        public void Save() => Service.PluginInterface.SavePluginConfig(this);
    }
}
