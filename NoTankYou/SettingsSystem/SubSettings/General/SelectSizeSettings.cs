using Dalamud.Interface.Components;
using ImGuiNET;
using NoTankYou.DisplaySystem;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class SelectSizeSettings : SettingsCategory
    {
        private int SizeRadioButton = 0;
        private int LastSizeRadioButton = -1;

        private int ScalePercent = 100;

        public SelectSizeSettings() : base("Banner Size Selection")
        {
            SizeRadioButton = (int)Service.Configuration.ImageSize;
            ScalePercent = (int) (Service.Configuration.GlobalScaleFactor * 100.0);

        }

        protected override void DrawContents()
        {
            //DrawSelectSize();

            DrawScaleSlider();
        }

        private void DrawSelectSize()
        {
            ImGui.RadioButton("Small", ref SizeRadioButton, 0); ImGui.SameLine();
            ImGui.RadioButton("Medium", ref SizeRadioButton, 1); ImGui.SameLine();
            ImGui.RadioButton("Large", ref SizeRadioButton, 2);

            ImGui.Spacing();

            Service.Configuration.ImageSize = (WarningBanner.ImageSize)SizeRadioButton;

            if (LastSizeRadioButton == SizeRadioButton) return;

            Service.Configuration.ForceWindowUpdate = true;
            LastSizeRadioButton = SizeRadioButton;
        }

        private void DrawScaleSlider()
        {
            ImGui.Text("Scale %%, Control + Click to input number directly");
            ImGui.SliderInt("", ref ScalePercent, -50, 250, "%d %%");
            ImGuiComponents.HelpMarker("Scales the banners by the specified percentage\n" +
                                       "0% - Original Size" +
                                       "100% - 2x Size" +
                                       "-50% - 1/2x Size");

            Service.Configuration.GlobalScaleFactor = ScalePercent / 100.0f;
            Service.Configuration.ForceWindowUpdate = true;
        }
    }
}
