using NoTankYou.Classes;
using NoTankYou.Windows;

namespace NoTankYou;

public static class System {
    public static ModuleBrowserWindow ConfigurationWindow { get; set; } = null!;
    public static ModuleManager ModuleManager { get; set; } = null!;
	public static SystemConfig? SystemConfig { get; set; }
    public static WarningController WarningController { get; set; } = null!;
}
