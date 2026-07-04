using System;
using System.Threading.Tasks;
using KamiToolKit.BaseTypes;
using KamiToolKit.UiOverlay;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.WarningBanner;

public class WarningBanner : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Warning Banner",
        FileName = "BannerDisplay",
        IconId = 0,
        Type = ModuleType.WarningDisplays,
    };

    private OverlayController? overlayController;

    public override NodeBase DisplayNode => new WarningBannerConfigNode(this);

    public WarningBannerConfig Config = null!;

    public static WarningBannerConfig? WarningBannerConfig { get; private set; }

    protected override async Task OnFeatureLoad() {
        Config = await Utilities.Config.LoadCharacterConfig<WarningBannerConfig>($"{ModuleInfo.FileName}.config.json");
        if (Config is null) throw new Exception("Failed to load config file");

        Config.FileName = ModuleInfo.FileName;
        WarningBannerConfig = Config;
    }

    protected override Task OnFeatureUnload() {
        Config = null!;
        WarningBannerConfig = null;

        return Task.CompletedTask;
    }

    protected override async Task OnFeatureEnable() {
        await Services.Framework.Run(() => {
            overlayController = new OverlayController();
            overlayController.AddNode(new WarningBannerOverlayNode {
                Position = Config.Position,
                Size = Config.Size,
                Config = Config,
            });
        });
    }

    protected override async Task OnFeatureDisable() {
        await Services.Framework.Run(() => overlayController?.Dispose());
        overlayController = null;
    }

    protected override void OnFeatureUpdate() {
        if (Config.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            Task.Run(Config.Save);
        }
    }
}
