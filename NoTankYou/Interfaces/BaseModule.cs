using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using NoTankYou.DataModels;
using NoTankYou.UserInterface.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Interfaces;

public abstract class BaseModule : IModule, IConfigurationComponent, ILogicComponent
{
    public IConfigurationComponent ConfigurationComponent => this;
    public ILogicComponent LogicComponent => this;
    public IModule ParentModule => this;
    public ISelectable Selectable => new ConfigurationSelectable(this);

    public abstract ModuleName Name { get; }
    public abstract string Command { get; }
    public abstract List<uint> ClassJobs { get; }
    public abstract GenericSettings GenericSettings { get; }

    protected virtual void DrawExtraConfiguration()
    {
        
    }
    
    public virtual void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(GenericSettings);
            
        DrawExtraConfiguration();
        
        InfoBox.Instance.DrawOverlaySettings(GenericSettings);
            
        InfoBox.Instance.DrawOptions(GenericSettings);
    }
    
    public abstract WarningState? EvaluateWarning(PlayerCharacter character);
}