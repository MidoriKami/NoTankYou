using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace NoTankYou;

public class Services {
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; set;} = null!;
    [PluginService] public static IFramework Framework { get; set;} = null!;
    [PluginService] public static IObjectTable ObjectTable { get; set;} = null!;
    [PluginService] public static IDutyState DutyState { get; set; } = null!;
    [PluginService] public static IGameGui GameGui { get; set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; set; } = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] public static ICondition Condition { get; set; } = null!;
    [PluginService] public static IDataManager DataManager { get; set; } = null!;
    [PluginService] public static IChatGui Chat { get; set; } = null!;
    [PluginService] public static IJobGauges JobGauges { get; set; } = null!;
    [PluginService] public static IPlayerState PlayerState { get; set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
    [PluginService] public static IUnlockState UnlockState { get; set; } = null!;
}
