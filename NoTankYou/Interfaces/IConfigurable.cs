using ImGuiNET;

namespace NoTankYou.Interfaces
{
    internal interface IConfigurable : ITabItem
    {
        string ConfigurationPaneLabel { get; }
        void DrawOptions();

        void DrawBaseOptions() => DrawOptions();

        void ITabItem.DrawConfigurationPane()
        {
            var contentWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(ConfigurationPaneLabel).X;
            var textStart = contentWidth / 2.0f - textWidth / 2.0f;

            ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = textStart});
            ImGui.Text(ConfigurationPaneLabel);

            ImGui.Spacing();

            DrawTabs();
        }

        void DrawTabs()
        {

            if (ImGui.BeginChild("OptionsContentsChild", ImGui.GetContentRegionAvail(), false,
                    ImGuiWindowFlags.AlwaysVerticalScrollbar))
            {
                DrawBaseOptions();
            }

            ImGui.EndChild();
        }
    }
}
