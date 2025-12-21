using System.Collections.Generic;
using NoTankYou.Classes;

namespace NoTankYou.Configuration;

public class BannerConfig {
    public bool Enabled = true;
    public bool SoloMode;
    public bool SampleMode; 
    public bool EnableAnimation = true;
    public bool EnableActionTooltip = true;

    public BannerOverlayDisplayMode DisplayMode = BannerOverlayDisplayMode.List;

    public HashSet<ModuleName> BlacklistedModules = [];
}