using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace NoTankYou.UserInterface.Components.InfoBox;

internal class InfoBoxTable
{
    private readonly InfoBox Owner;
    private readonly float Weight;

    private readonly List<Tuple<Action?, Action?>> TableRows = new();

    public InfoBoxTable(InfoBox owner, float weight = 0.5f)
    {
        this.Owner = owner;
        this.Weight = weight;
    }

    public InfoBoxTable AddRow(string label, string contents, Vector4? firstColor = null, Vector4? secondColor = null)
    {
        TableRows.Add(new Tuple<Action?, Action?>(
            Actions.GetStringAction(label, firstColor), 
            Actions.GetStringAction(contents, secondColor)
        ));

        return this;
    }

    public InfoBoxTable AddActions(Action? firstAction, Action? secondAction)
    {
        TableRows.Add(new Tuple<Action?, Action?>(firstAction, secondAction));

        return this;
    }

    public InfoBox EndTable()
    {
        Owner.AddAction(() =>
        {
            if (ImGui.BeginTable($"", 2, ImGuiTableFlags.None, new Vector2(Owner.InnerWidth, 0)))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (Weight) );
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 1f * (1 - Weight) );

                foreach (var row in TableRows)
                {
                    ImGui.TableNextColumn();

                    ImGui.PushTextWrapPos(GetWrapPosition());
                    row.Item1?.Invoke();
                    ImGui.PopTextWrapPos();

                    ImGui.TableNextColumn();
                    ImGui.PushTextWrapPos(GetWrapPosition());
                    row.Item2?.Invoke();
                    ImGui.PopTextWrapPos();
                }

                ImGui.EndTable();
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

    public InfoBoxTable AddRows(IEnumerable<Tuple<Action?, Action?>> rows)
    {
        foreach (var row in rows)
        {
            TableRows.Add(row);
        }

        return this;
    }
}