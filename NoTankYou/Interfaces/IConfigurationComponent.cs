
using KamiLib.Interfaces;

namespace NoTankYou.Interfaces;

public interface IConfigurationComponent : IDrawable
{
    ISelectable Selectable { get; }
}