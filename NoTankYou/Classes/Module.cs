using System;
using NoTankYou.Utilities;

namespace NoTankYou.Classes;

public abstract class Module<T> : ModuleBase where T : ConfigBase, new() {
    public sealed override ConfigBase ConfigBase => ModuleConfig;

    public T ModuleConfig { get; private set; } = null!;

    protected override void OnFeatureLoad() {
        ModuleConfig = Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");

        ModuleConfig.FileName = ModuleInfo.FileName;
    }

    protected override void OnFeatureUnload() {
        ModuleConfig = null!;
    }
}
