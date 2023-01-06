using System;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Components;

public class ConfigurationSelectable : ISelectable
{
    private ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    private IModule ParentModule { get; }
    public string ID => ParentModule.Name.GetTranslatedString();

    public ConfigurationSelectable(IModule parentModule, IDrawable contents)
    {
        OwnerModuleName = parentModule.Name;
        ParentModule = parentModule;
        Contents = contents;
    }

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