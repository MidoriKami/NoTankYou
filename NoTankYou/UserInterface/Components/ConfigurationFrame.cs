using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Utilities;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Components;

internal class ConfigurationFrame
{
    public void Draw(ISelectable? selected)
    {
        DrawVerticalLine();
        ImGuiHelpers.ScaledDummy(0.0f);
        ImGui.SameLine();

        var regionAvailable = ImGui.GetContentRegionAvail();

        if (ImGui.BeginChild("###ConfigurationFrame", new Vector2(regionAvailable.X, 0), false, ImGuiWindowFlags.AlwaysVerticalScrollbar))
        {
            if (selected != null)
            {
                selected.Contents.Draw();
            }
            else
            {
                var available = ImGui.GetContentRegionAvail() / 2.0f;
                var textSize = ImGui.CalcTextSize(Strings.Configuration.NoSelection) / 2.0f;
                var center = new Vector2(available.X - textSize.X, available.Y - textSize.Y);

                ImGui.SetCursorPos(center);
                ImGui.TextWrapped(Strings.Configuration.NoSelection);
            }
        }

        ImGui.EndChild();
    }

    private void DrawVerticalLine()
    {
        var contentArea = ImGui.GetContentRegionAvail();
        var cursor = ImGui.GetCursorScreenPos();
        var drawList = ImGui.GetWindowDrawList();
        var color = ImGui.GetColorU32(Colors.White);

        drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
    }
}