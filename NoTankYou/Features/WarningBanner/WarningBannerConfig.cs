using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.WarningBanner;

public class WarningBannerConfig : Savable {
    public bool SoloMode = false;

    public BannerDisplayMode DisplayMode = BannerDisplayMode.List;

    public Vector2 Position = new(500.0f, 500.0f);
    public float Scale = 1.0f;
    public Vector2 Size = new(300.0f, 200.0f);

    public bool ShowWarningShield = true;
    public bool ShowWarningText = true;
    public bool ShowPlayerName = true;
    public bool ShowActionName = true;
    public bool ShowActionIcon = true;
    public bool EnableAnimation = true;
    public bool EnableActionTooltip = true;

    public List<string> DisabledModules = [];

    [JsonIgnore] public bool EnableMoving;
    [JsonIgnore] public bool EnableResizing;

    protected override string FileExtension => ".config.json";
}
