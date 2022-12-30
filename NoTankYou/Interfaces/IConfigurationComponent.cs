
using KamiLib.Interfaces;

namespace NoTankYou.Interfaces;

public interface IConfigurationComponent : IDrawable
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }
}