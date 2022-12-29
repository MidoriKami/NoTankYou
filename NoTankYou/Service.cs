using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace NoTankYou;

public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static PartyList PartyList { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;
    [PluginService] public static DataManager DataManager { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;

    public static WindowSystem WindowSystem { get; set; } = new("NoTankYou");

    internal static System.ModuleManager ModuleManager = null!;
    internal static System.LocalizationManager LocalizationManager = null!;
    internal static System.ConfigurationManager ConfigurationManager = null!;
    internal static System.FontManager FontManager = null!;
    internal static System.DutyEventManager DutyEventManager = null!;
    internal static System.ContextManager ContextManager = null!;
    internal static System.PartyListAddon PartyListAddon = null!;
    internal static System.DutyLists DutyLists = null!;
}