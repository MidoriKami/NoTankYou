﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace NoTankYou.SettingsSystem
{
    internal abstract class BannerSettings : SettingsCategory
    {
        protected abstract ref Configuration.ModuleSettings Settings { get; }

        private int ScalePercent = 100;
        private int LastScalePercent = 100;

        protected BannerSettings(string categoryName) : base(categoryName)
        {
            ScalePercent = (int) (Settings.ScaleFactor * 100);
            LastScalePercent = ScalePercent;
        }

        protected override void DrawContents()
        {
            DrawGenericSettings();

            DrawComponentControl();

            DrawScaleSettings();
        }

        private void DrawScaleSettings()
        {
            ImGui.Text("Scaling Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Text("Scale %%, Control + Click to input number directly");
            ImGui.SliderInt("", ref ScalePercent, 50, 250, "%d %%");
            ImGuiComponents.HelpMarker("Scales the banners by the specified percentage\n" +
                                       "0% - Original Size" +
                                       "100% - 2x Size" +
                                       "-50% - 1/2x Size");

            if (LastScalePercent != ScalePercent)
            {
                Settings.ScaleFactor = ScalePercent / 100.0f;
                Service.Configuration.ForceWindowUpdate = true;
                LastScalePercent = ScalePercent;
            }

            ImGui.Spacing();
        }

        private void DrawGenericSettings()
        {
            ImGui.Text("General Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Enable Warning", ref Settings.Enabled);
            ImGui.Spacing();

            ImGui.Checkbox("Force Show Banner", ref Settings.Forced);
            ImGui.Spacing();

            ImGui.Checkbox("Reposition Banner", ref Settings.Reposition);
            ImGui.Spacing();
        }
        private void DrawComponentControl()
        {
            ImGui.Text("Image Component Settings");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.Checkbox("Show Exclamation Mark", ref Settings.ShowShield);
            ImGui.Spacing();

            ImGui.Checkbox("Show Text", ref Settings.ShowText);
            ImGui.Spacing();

            ImGui.Checkbox("Show Icon", ref Settings.ShowIcon);
            ImGui.Spacing();
        }
    }
}
