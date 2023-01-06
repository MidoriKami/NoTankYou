using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using NoTankYou.System;

namespace NoTankYou;

public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set;} = null!;
    [PluginService] public static PartyList PartyList { get; private set;} = null!;
    [PluginService] public static Framework Framework { get; private set;} = null!;
    [PluginService] public static ObjectTable ObjectTable { get; private set;} = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;

    internal static ModuleManager ModuleManager = null!;
    internal static ConfigurationManager ConfigurationManager = null!;
    internal static FontManager FontManager = null!;
    internal static PartyListAddon PartyListAddon = null!;
}