using System;
using Dalamud.Interface;
using ImGuiNET;

namespace NoTankYou.SettingsSystem
{
    internal abstract class DisplayModule : IDisposable
    {
        public string CategoryString = "CategoryString Not Set";

        protected abstract void DrawContents();
        
        public virtual void Draw()
        {
            if (ImGui.CollapsingHeader(CategoryString))
            {
                ImGui.Indent(20 * ImGuiHelpers.GlobalScale);

                ImGui.Spacing();

                DrawContents();

                ImGui.Indent(-20 * ImGuiHelpers.GlobalScale);
            }
        }

        public abstract void Dispose();
    }
}
