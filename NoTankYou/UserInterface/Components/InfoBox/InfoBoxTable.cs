using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Components.InfoBox;

public class InfoBoxTable
{
    private readonly InfoBox Owner;
    private readonly float Weight;

    private readonly List<InfoBoxTableRow> Rows = new();
    private readonly string EmptyListString = string.Empty;

    public InfoBoxTable(InfoBox owner, float weight = 0.5f)
    {
        this.Owner = owner;
        this.Weight = weight;
    }

    public InfoBoxTableRow BeginRow()
    {
        return new InfoBoxTableRow(this);
    }

    public InfoBoxTable AddRow(InfoBoxTableRow row)
    {
        Rows.Add(row);

        return this;
    }

    public InfoBox EndTable()
    {
        Owner.AddAction(() =>
        {
            if (Rows.Count == 0)
            {
                if (EmptyListString != string.Empty)
                {
                    ImGui.TextColored(Colors.Orange, EmptyListString);
                }
            }
            else
            {
                if (ImGui.BeginTable($"", 2, ImGuiTableFlags.None, new Vector2(Owner.InnerWidth, 0)))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (Weight));
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (1 - Weight));

                    foreach (var row in Rows)
                    {
                        ImGui.TableNextColumn();

                        ImGui.PushTextWrapPos(GetWrapPosition());
                        row.FirstColumn?.Invoke();
                        ImGui.PopTextWrapPos();

                        ImGui.TableNextColumn();
                        ImGui.PushTextWrapPos(GetWrapPosition());
                        row.SecondColumn?.Invoke();
                        ImGui.PopTextWrapPos();
                    }

                    ImGui.EndTable();
                }

            }
        });

        return Owner;
    }

    private float GetWrapPosition()
    {
        var region = ImGui.GetContentRegionAvail();

        var cursor = ImGui.GetCursorPos();

        var wrapPosition = cursor.X + region.X;

        return wrapPosition;
    }
}