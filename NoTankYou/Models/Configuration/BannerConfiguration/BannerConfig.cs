using System.Collections.Generic;
using System.Numerics;
using NoTankYou.DataModels;
using NoTankYou.Models.BannerConfiguration;
using NoTankYou.Models.Enums;

namespace NoTankYou.Models;

public class BannerConfig : IBannerMainOptions, IBannerPositioning, IBannerDisplayOptions, IBannerDisplayStyle, IBannerModuleBlacklist
{
    // IBannerMainOptions
    public bool Enabled { get; set; } = true;
    public bool SoloMode { get; set; } = false;
    public bool SampleMode { get; set; } = false;
    
    // IBannerPositioning
    public bool CanDrag { get; set; } = false;
    public Vector2 WindowPosition { get; set; } = new(700.0f, 400.0f);
    public float Scale { get; set; } = 1.0f;
    
    // IBannerDisplayOptions
    public BannerOverlayDisplayMode DisplayMode { get; set; } = BannerOverlayDisplayMode.List;
    public int WarningCount { get; set; } = 10;
    public float AdditionalSpacing { get; set; } = 0.0f;
    
    // IBannerDisplayStyle
    public bool WarningShield { get; set; } = true;
    public bool WarningText { get; set; } = true; 
    public bool PlayerNames { get; set; } = true;
    public bool ShowActionName { get; set; } = true;
    public bool Icon { get; set; } = true;
    
    // IBannerModuleBlacklist
    public HashSet<ModuleName> BlacklistedModules { get; set; } = new();
}