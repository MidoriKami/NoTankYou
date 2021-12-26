using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.DisplaySystem;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class SelectSizeSettings : SettingsCategory
    {
        private int ScalePercent = 100;
        private int LastScalePercent = 100;

        public SelectSizeSettings() : base("Banner Size Selection")
        {
            ScalePercent = (int) (Service.Configuration.GlobalScaleFactor * 100.0) + 100;
            LastScalePercent = ScalePercent;
        }

        protected override void DrawContents()
        {
            DrawScaleSlider();
        }

        private void DrawScaleSlider()
        {
            ImGui.Text("Scale %%, Control + Click to input number directly");
            ImGui.SliderInt("", ref ScalePercent, 50, 250, "%d %%");
            ImGuiComponents.HelpMarker("Scales the banners by the specified percentage");

            if (LastScalePercent != ScalePercent)
            {
                Service.Configuration.GlobalScaleFactor = (ScalePercent - 100) / 100.0f;
                Service.Configuration.ForceWindowUpdate = true;

                LastScalePercent = ScalePercent;
            }
        }
    }
}
