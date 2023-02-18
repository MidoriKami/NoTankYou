
using KamiLib.Interfaces;

namespace NoTankYou.Interfaces;

public interface IConfigurationComponent
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }
    
    void DrawConfiguration();
}