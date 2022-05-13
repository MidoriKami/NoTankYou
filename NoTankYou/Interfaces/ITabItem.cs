using NoTankYou.Enums;

namespace NoTankYou.Interfaces
{
    internal interface ITabItem
    {
        ModuleType ModuleType { get; }

        void DrawTabItem();

        void DrawConfigurationPane();
    }
}
