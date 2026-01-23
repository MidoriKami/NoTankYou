using System;
using KamiToolKit;
using KamiToolKit.Overlay;
using NoTankYou.Classes;
using NoTankYou.Enums;
using NoTankYou.Windows;

namespace NoTankYou.Features.WarningBanner;

public class WarningBanner : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Warning Banner",
        FileName = "BannerDisplay",
        IconId = 0,
        Type = ModuleType.GeneralFeatures,
    };

    private MultiSelectWindow? moduleSelectionWindow;
    private OverlayController? overlayController;
    
    public override NodeBase DisplayNode => new WarningBannerConfigNode(this);

    public WarningBannerConfig Config = null!;

    public static WarningBannerConfig? WarningBannerConfig;

    protected override void OnFeatureLoad() {
        Config = Utilities.Config.LoadCharacterConfig<WarningBannerConfig>($"{ModuleInfo.FileName}.config.json");
        if (Config is null) throw new Exception("Failed to load config file");
        
        Config.FileName = ModuleInfo.FileName;
        WarningBannerConfig = Config;
    }

    protected override void OnFeatureUnload() {
        Config = null!;
        WarningBannerConfig = null;
    }

    protected override void OnFeatureEnable() {
        overlayController = new OverlayController();

        overlayController.CreateNode(() => new WarningBannerOverlayNode {
            Position = Config.Position,
            Size = Config.Size,
            Config = Config,
        });
    }

    protected override void OnFeatureDisable() {
        moduleSelectionWindow?.Dispose();
        moduleSelectionWindow = null;
        
        overlayController?.Dispose();
        overlayController = null;
    }
    
    protected override void OnFeatureUpdate() {
        if (Config.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            Config.Save();
        }
    }
}
