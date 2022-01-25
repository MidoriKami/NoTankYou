using System;
using System.Collections.Generic;
using Dalamud.Interface;
using ImGuiNET;

namespace NoTankYou.SettingsSystem
{
    internal abstract class TabCategory : IDisposable
    {
        protected List<DisplayModule> Modules = new();

        public string CategoryName { get; protected set; } = "Unset CategoryName";
        public string TabName { get; protected set; } = "Unset TabName";

        protected virtual void DrawContents()
        {

        }

        public void Draw()
        {
            ImGui.Text(CategoryName);
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.BeginChild(CategoryName, ImGuiHelpers.ScaledVector2(530, 500), true);

            foreach (var module in Modules)
            {
                module.Draw();
            }

            ImGui.EndChild();

            ImGui.Spacing();
        }

        public void Dispose()
        {
            foreach (var module in Modules)
            {
                module.Dispose();
            }
        }
    }
}
