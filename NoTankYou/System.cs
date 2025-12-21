using System.Collections.Generic;
using KamiLib.CommandManager;
using KamiLib.DebugWindows;
using KamiLib.Window;
using NoTankYou.Classes;
using NoTankYou.Configuration;
using NoTankYou.Controllers;
using NoTankYou.Windows;

namespace NoTankYou;

public static class System {
	public static ModuleController ModuleController { get; set; } = null!;
	public static BannerController BannerController { get; set; } = null!;
	public static PartyListController PartyListController { get; set; } = null!;
	public static SystemConfig? SystemConfig { get; set; }
	public static BannerConfig? BannerConfig { get; set; }
	public static BannerListStyle? BannerListStyle { get; set; }
	public static BannerStyle? BannerStyle { get; set; }
	public static BlacklistController BlacklistController { get; set; } = null!;
	public static List<WarningState> ActiveWarnings { get; set; } = [];
	public static WindowManager WindowManager { get; set; } = null!;
	public static CommandManager CommandManager { get; set; } = null!;
	public static ConfigurationWindow ConfigurationWindow { get; set; } = null!;
	public static DutyTypeDebugWindow DutyTypeDebugWindow { get; set; } = null!;
}