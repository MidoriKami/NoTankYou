using System;
using System.Threading.Tasks;
using NoTankYou.Utilities;

namespace NoTankYou.Classes;

public abstract class Module<T> : ModuleBase where T : ConfigBase, new() {
    public sealed override ConfigBase ConfigBase => ModuleConfig;

    public T ModuleConfig { get; private set; } = null!;

    protected sealed override async Task OnFeatureLoad() {
        ModuleConfig = await Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");

        ModuleConfig.FileName = ModuleInfo.FileName;

        await base.OnFeatureLoad();
    }

    protected sealed override Task OnFeatureUnload() {
        ModuleConfig = null!;

        return Task.CompletedTask;
    }
}
