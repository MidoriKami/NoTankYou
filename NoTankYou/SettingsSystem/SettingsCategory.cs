using ImGuiNET;

namespace NoTankYou.SettingsSystem
{
    internal abstract class SettingsCategory
    {
        protected readonly string CategoryName;

        protected SettingsCategory(string categoryName)
        {
            this.CategoryName = categoryName;
        }

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
