using ImGuiNET;
using NoTankYou.DisplaySystem;

namespace NoTankYou.SettingsSystem.SubSettings.General
{
    internal class SelectSizeSettings : SettingsCategory
    {
        private int SizeRadioButton = 0;
        private int LastSizeRadioButton = -1;

        public SelectSizeSettings() : base("Banner Size Selection")
        {
            SizeRadioButton = (int)Service.Configuration.ImageSize;
        }

        protected override void DrawContents()
        {
            DrawSelectSize();
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
    }
}
