using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;
using NoTankYou.Classes;
using NoTankYou.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IAsyncDalamudPlugin {
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public Task LoadAsync(CancellationToken cancellationToken) {
        KamiToolKitLibrary.Initialize(PluginInterface, "NoTankYou");

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "NoTankYouConfig",
            Title = "NoTankYou Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };

        ICommandManager.Get().AddHandler(["/notankyou", "/nty"], new CommandInfo(OnCommand) {
            HelpMessage = "Open NoTankYou config window",
        });

        System.WarningController = new WarningController();
        System.ModuleManager = new ModuleManager();

        if (IClientState.Get().IsLoggedIn) {
            IFramework.Get().RunOnFrameworkThread(OnLogin);
        }

        IClientState.Get().Login += OnLogin;
        IClientState.Get().Logout += OnLogout;

        PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi += System.ConfigurationWindow.Toggle;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() {
        PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        IClientState.Get().Login -= OnLogin;
        IClientState.Get().Logout -= OnLogout;

        if (!IFramework.Get().IsFrameworkUnloading) {
            await System.ConfigurationWindow.DisposeAsync();
            await System.ModuleManager.DisposeAsync();
        }

        await IFramework.Get().RunOnFrameworkThread(KamiToolKitLibrary.Dispose);
    }

    private static void OnCommand(string command, string arguments) {
        if (command is not ("/notankyou" or "/nty")) return;

        switch (arguments) {
            case null or "":
                if (!IClientState.Get().IsLoggedIn) return;
                System.ConfigurationWindow.Open();
                break;
        }
    }

    private void OnLogin() {
        Task.Run(async () => {
            System.SystemConfig = await SystemConfig.Load();
            await System.ModuleManager.LoadModules();
        });
    }

    private void OnLogout(int type, int code) {
        Task.Run(async () => {
            await System.ModuleManager.UnloadModules();
            System.SystemConfig = null;
        });
    }
}
