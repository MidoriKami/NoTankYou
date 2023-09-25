using System.Drawing;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using NoTankYou.Abstracts;
using NoTankYou.Localization;

namespace NoTankYou.Views.Components;

public class ModuleSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    public string ID => module.ModuleName.Label();

    private readonly ModuleBase module;
    
    public ModuleSelectable(ModuleBase module)
    {
        this.module = module;
    }
    
    public void DrawLabel()
    {
        ImGui.TextUnformatted(module.ModuleName.Label());
        
        var region = ImGui.GetContentRegionAvail();
        
        var textColor = module.ModuleConfig.Enabled ? KnownColor.ForestGreen.Vector() : KnownColor.OrangeRed.Vector();
        var text = module.ModuleConfig.Enabled ? Strings.Enabled : Strings.Disabled;

        var textSize = ImGui.CalcTextSize(text);
        
        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(textColor, text);
    }
 
    public void Draw()
    {
        module.DrawConfig();
    }
}