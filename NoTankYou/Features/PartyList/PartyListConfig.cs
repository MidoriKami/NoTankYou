using System.Collections.Generic;
using System.Numerics;
using NoTankYou.Classes;

namespace NoTankYou.Features.PartyList;

public class PartyListConfig : Savable {
    public bool SoloMode = false;
    public bool Animation = true;
    public bool Tooltips = true;
    public bool UseModuleIcons = false;
    public bool ShowGlow = true;
    public bool ShowIcon = true;
    public Vector4 GlowColor = new(0.90f, 0.5f, 0.5f, 1.0f);
    
    public List<string> DisabledModules = [];

    protected override string FileExtension => ".config.json";
}
