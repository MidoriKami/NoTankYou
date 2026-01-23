using System;
using KamiToolKit;

namespace NoTankYou.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;
    
    public Action? OpenConfigAction { get; set; }
    public abstract NodeBase DisplayNode { get; }
    
    protected bool IsEnabled { get; private set; }

    public void Load()
        => OnFeatureLoad();

    public void Unload()
        => OnFeatureUnload();

    public void Enable() {
        IsEnabled = true;
        
        OnFeatureEnable();
    }

    public void Disable() {
        IsEnabled = false;
        
        OnFeatureDisable();
    }

    public void Update()
        => OnFeatureUpdate();
    
    private void TerritoryChanged(ushort obj)
        => OnTerritoryChanged();

    protected abstract void OnFeatureUpdate();
    protected virtual void OnTerritoryChanged() { }

    protected abstract void OnFeatureLoad();
    protected abstract void OnFeatureUnload();
    
    protected abstract void OnFeatureEnable();
    protected abstract void OnFeatureDisable();
}
