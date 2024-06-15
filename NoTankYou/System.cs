using System.Collections.Generic;
using KamiLib.CommandManager;
using KamiLib.Window;
using NoTankYou.Classes;
using NoTankYou.Controllers;
using NoTankYou.Windows;

namespace NoTankYou;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System {
	public static ModuleController ModuleController { get; set; }
	public static BannerController BannerController { get; set; }
	public static PartyListController PartyListController { get; set; }
	public static SystemConfig SystemConfig { get; set; }
	public static BlacklistController BlacklistController { get; set; }
	public static List<WarningState> ActiveWarnings { get; set; } = [];
	public static LocalizationController LocalizationController { get; set; }
	public static WindowManager WindowManager { get; set; }
	public static CommandManager CommandManager { get; set; }
	
	public static ConfigurationWindow ConfigurationWindow { get; set; }
}