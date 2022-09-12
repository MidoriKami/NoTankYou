using System;
using ImGuiNET;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Components;

internal class ConfigurationSelectable : ISelectable
{
    public ModuleName OwnerModuleName { get; }
    public IDrawable Contents { get; }
    public IModule ParentModule { get; }

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

        var text = ParentModule.GenericSettings.Enabled.Value ? Strings.Common.Labels.Enabled : Strings.Common.Labels.Disabled;
        var color = ParentModule.GenericSettings.Enabled.Value ? Colors.Green : Colors.Red;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
}