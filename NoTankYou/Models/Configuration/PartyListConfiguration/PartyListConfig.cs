using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiLib.Utilities;
using NoTankYou.Models.BannerConfiguration;
using NoTankYou.Models.Enums;

namespace NoTankYou.Models;

public class PartyListConfig : IPartyListMainOptions, IPartyListDisplayStyle, IPartyListDisplayColors, IPartyListBlacklist
{
    // IPartyListMainOptions
    public bool Enabled { get; set; } = true;
    public bool SoloMode { get; set; } = false;
    public bool SampleMode { get; set; } = false;
    
    // IPartyListDisplayStyle
    public bool WarningText { get; set; } = true;
    public bool PlayerName { get; set; } = true;
    public bool JobIcon { get; set; } = true;
    public bool Animation { get; set; } = true;
    public float AnimationPeriod { get; set; } = 1000;
    
    // IPartyListDisplayColors
    public Vector4 TextColor { get; set; } = KnownColor.Red.Vector();
    public Vector4 OutlineColor { get; set; } = KnownColor.Red.Vector();
    
    // IPartyListBlacklist
    public HashSet<ModuleName> BlacklistedModules { get; set; } = new();
}