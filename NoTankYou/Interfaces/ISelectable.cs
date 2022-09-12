using NoTankYou.Configuration.Enums;

namespace NoTankYou.Interfaces;

internal interface ISelectable
{
    ModuleName OwnerModuleName { get; }
    IDrawable Contents { get; }
    IModule ParentModule { get; }

    void DrawLabel();
}