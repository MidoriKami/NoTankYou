using System;
using ImGuiNET;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Components;

public class ConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;
    
    private readonly IConfigurationComponent configurationComponent;
    private ModuleName OwnerModuleName => configurationComponent.ParentModule.Name;
    private IModule ParentModule => configurationComponent.ParentModule;
    public string ID => ParentModule.Name.GetTranslatedString();

    public ConfigurationSelectable(IConfigurationComponent parentModule)
    {
        configurationComponent = parentModule;
    }

    public void Draw() => configurationComponent.DrawConfiguration();

    public void DrawLabel()
    {
        DrawModuleLabel();
        DrawModuleStatus();
    }

    private void DrawModuleLabel()
    {
        ImGui.Text(OwnerModuleName.GetTranslatedString()[..Math.Min(OwnerModuleName.GetTranslatedString().Length, 20)]);
    }

    private void DrawModuleStatus()
    {
        var region = ImGui.GetContentRegionAvail();

        var text = ParentModule.GenericSettings.Enabled ? Strings.Labels_Enabled : Strings.Labels_Disabled;
        var color = ParentModule.GenericSettings.Enabled ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}