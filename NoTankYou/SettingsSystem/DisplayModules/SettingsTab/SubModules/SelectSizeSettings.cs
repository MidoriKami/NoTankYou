using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules
{
    internal class SelectSizeSettings : SettingsCategory
    {
        private int ScalePercent = 100;
        private int LastScalePercent = 100;

        public SelectSizeSettings() : base("Banner Size Selection")
        {
            ScalePercent = (int) (Service.Configuration.GlobalScaleFactor * 100.0);
            LastScalePercent = ScalePercent;
        }

        protected override void DrawContents()
        {
            DrawScaleSlider();
        }

        private void DrawScaleSlider()
        {
            if (ImGui.Checkbox("Override Individual Scale Settings",
                    ref Service.Configuration.OverrideIndividualScaleSettings))
            {
                Service.Configuration.GlobalScaleFactor = (ScalePercent) / 100.0f;
                Service.Configuration.ForceWindowUpdate = true;

                LastScalePercent = ScalePercent;
            }

            if (Service.Configuration.OverrideIndividualScaleSettings)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                ImGui.Text("Scale %%, Control + Click to input number directly");
                ImGui.SliderInt("", ref ScalePercent, 50, 250, "%d %%");
                ImGuiComponents.HelpMarker("Scales the banners by the specified percentage");

                if (LastScalePercent != ScalePercent)
                {
                    Service.Configuration.GlobalScaleFactor = (ScalePercent) / 100.0f;
                    Service.Configuration.ForceWindowUpdate = true;

                    LastScalePercent = ScalePercent;
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }
        }
    }
}
