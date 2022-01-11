using ImGuiNET;

namespace NoTankYou.SettingsSystem
{
    internal abstract class TabCategory
    {
        public string CategoryName { get; protected set; } = "Unset CategoryName";
        public string TabName { get; protected set; } = "Unset TabName";

        protected abstract void DrawContents();

        public void Draw()
        {
            ImGui.Text(CategoryName);
            ImGui.Separator();
            ImGui.Spacing();

            DrawContents();

            ImGui.Spacing();
        }
    }
}
