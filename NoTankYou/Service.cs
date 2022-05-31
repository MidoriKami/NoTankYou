using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using NoTankYou.Data;
using NoTankYou.System;

namespace NoTankYou
{
    public class Service
    {
        [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static ChatGui Chat { get; private set; } = null!;
        [PluginService] public static ClientState ClientState { get; private set; } = null!;
        [PluginService] public static PartyList PartyList { get; private set; } = null!;
        [PluginService] public static CommandManager Commands { get; private set; } = null!;
        [PluginService] public static Condition Condition { get; private set; } = null!;
        [PluginService] public static DataManager DataManager { get; private set; } = null!;
        [PluginService] public static Framework Framework { get; private set; } = null!;
        [PluginService] public static ObjectTable ObjectTable { get; private set; } = null!;
        [PluginService] public static GameGui GameGui { get; private set; } = null!;

        public static WindowSystem WindowSystem { get; set; } = new("NoTankYou");
        public static WindowManager WindowManager { get; set; } = null!;
        public static ModuleManager ModuleManager { get; set; } = null!;
        public static EventManager EventManager { get; set; } = null!;
        public static HudManager HudManager { get; set; } = null!;
        public static Dalamud.Localization Localization { get; set; } = null!;
        public static IconManager IconManager { get; set; } = null!;
        public static FontManager FontManager { get; set; } = null!;
        public static ContextManager ContextManager { get; set; } = null!;
        public static Configuration Configuration { get; set; } = null!;
    }
}