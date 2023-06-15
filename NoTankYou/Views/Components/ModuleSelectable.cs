using System.Drawing;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using NoTankYou.Abstracts;

namespace NoTankYou.Views.Components;

public class ModuleSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    public string ID => module.ModuleName.GetLabel();

    private readonly ModuleBase module;
    
    public ModuleSelectable(ModuleBase module)
    {
        this.module = module;
    }
    
    public void DrawLabel()
    {
        var textColor = module.ModuleConfig.Enabled ? KnownColor.ForestGreen.AsVector4() : KnownColor.OrangeRed.AsVector4();
        var text = module.ModuleName.GetLabel();
        
        ImGui.TextColored(textColor, text);
    }
 
    public void Draw()
    {
        module.DrawConfig();
    }
}