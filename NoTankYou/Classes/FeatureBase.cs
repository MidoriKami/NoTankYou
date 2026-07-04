using System;
using System.Threading.Tasks;
using KamiToolKit.BaseTypes;

namespace NoTankYou.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;

    public Action? OpenConfigAction { get; set; }
    public abstract NodeBase DisplayNode { get; }

    protected bool IsEnabled { get; private set; }

    public async Task Load()
        => await OnFeatureLoad();

    public async Task Unload()
        => await OnFeatureUnload();

    public async Task Enable() {
        IsEnabled = true;

        await OnFeatureEnable();
    }

    public async Task Disable() {
        IsEnabled = false;

        await OnFeatureDisable();
    }

    public void Update()
        => OnFeatureUpdate();

    private void TerritoryChanged(ushort obj)
        => OnTerritoryChanged();

    protected abstract void OnFeatureUpdate();
    protected virtual void OnTerritoryChanged() { }

    protected abstract Task OnFeatureLoad();
    protected abstract Task OnFeatureUnload();

    protected abstract Task OnFeatureEnable();
    protected abstract Task OnFeatureDisable();
}
