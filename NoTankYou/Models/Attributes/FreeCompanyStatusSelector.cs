using System.Linq;
using System.Reflection;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace NoTankYou.Models.Attributes;

public class FreeCompanyStatusSelector : RightLabeledTabledDrawableAttribute
{
    public FreeCompanyStatusSelector(string? label) : base(label) { }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var value = GetValue<uint>(obj, field);
        var statusEffect = LuminaCache<Status>.Instance.GetRow(value)!;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.BeginCombo($"##{field.Name}", statusEffect.Name.ToDalamudString().ToString()))
        {
            foreach (var status in LuminaCache<Status>.Instance.Where(status => status.IsFcBuff))
            {
                if (ImGui.Selectable(status.Name.ToDalamudString().ToString(), value == status.RowId))
                {
                    SetValue(obj, field, status.RowId);
                    saveAction?.Invoke();
                }
            }
            
            ImGui.EndCombo();
        }
    }
}