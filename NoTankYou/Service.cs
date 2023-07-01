using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace NoTankYou;

public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set;} = null!;
    [PluginService] public static Framework Framework { get; private set;} = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set;} = null!;
    [PluginService] public static IDutyState DutyState { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
}