using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using KamiToolKit;
using NoTankYou.Classes;
using NoTankYou.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IAsyncDalamudPlugin {
    [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;

    public Task LoadAsync(CancellationToken cancellationToken) {
        PluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(PluginInterface, "NoTankYou");

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "NoTankYouConfig",
            Title = "NoTankYou Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };

        Services.CommandManager.AddHandler(["/notankyou", "/nty"], new CommandInfo(OnCommand) {
            HelpMessage = "Open NoTankYou config window",
        });

        System.WarningController = new WarningController();
        System.ModuleManager = new ModuleManager();

        if (Services.ClientState.IsLoggedIn) {
            Services.Framework.RunOnFrameworkThread(OnLogin);
        }

        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;

        Services.PluginInterface.UiBuilder.OpenConfigUi += System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi += System.ConfigurationWindow.Toggle;

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() {
        Services.PluginInterface.UiBuilder.OpenConfigUi -= System.ConfigurationWindow.Toggle;
        Services.PluginInterface.UiBuilder.OpenMainUi -= System.ConfigurationWindow.Toggle;

        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;

        await System.ConfigurationWindow.DisposeAsync();
        await System.ModuleManager.DisposeAsync();
        await Services.Framework.Run(KamiToolKitLibrary.Dispose);
    }

    private static void OnCommand(string command, string arguments) {
        if (command is not ("/notankyou" or "/nty")) return;

        switch (arguments) {
            case null or "":
                if (!Services.ClientState.IsLoggedIn) return;
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
