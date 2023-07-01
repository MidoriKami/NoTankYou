using System;
using System.Collections.Generic;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.AutomaticUserInterface;
using KamiLib.Utilities;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;

namespace NoTankYou.Models.Attributes;

public class ModuleBlacklistAttribute : DrawableAttribute
{
    public ModuleBlacklistAttribute(string? label) : base(label) { }
    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var hashSet = GetValue<HashSet<ModuleName>>(obj, field);

        ImGui.TextUnformatted(Strings.ModuleBlacklistInfo);
        ImGuiHelpers.ScaledDummy(10.0f);

        ImGui.Columns(2);

        foreach (var module in Enum.GetValues<ModuleName>()[..^1]) // Trim "Test" Module
        {
            var inHashset = hashSet.Contains(module);
            if(ImGui.Checkbox(module.GetLabel(), ref inHashset))
            {
                if (!inHashset) hashSet.Remove(module);
                if (inHashset) hashSet.Add(module);
                SetValue(obj, field, hashSet);
                saveAction?.Invoke();
            }
            ImGui.NextColumn();
        }
        
        ImGui.Columns(1);
    }
}