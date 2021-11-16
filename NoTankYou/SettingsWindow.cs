using ImGuiNET;
using System;
using System.Numerics;

using Dalamud.Interface.Windowing;

namespace NoTankYou
{
    internal class SettingsWindow : Window, IDisposable
    {
        public bool visible = false;

        private Vector2 WindowSize { get; set; }

        public SettingsWindow() : 
            base("Settings Window")
        {
            WindowSize = new Vector2(350, 250);

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize;
        }

        public void Dispose()
        {
            
        }

        private void DrawForceShowBannerCheckbox()
        {
            ImGui.Text("Force show 'Tank Stance' warning banner");
            ImGui.Checkbox("Force show warning banner", ref Service.Configuration.ForceShowNoTankWarning);
            ImGui.Spacing();
        }

        private void DrawEnablePluginButton()
        {
            ImGui.Checkbox("Enable Plugin", ref Service.Configuration.ShowNoTankWarning);
            ImGui.Spacing();
        }

        private void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Hide warning banner on map change for (milliseconds)");
            ImGui.InputInt("", ref Service.Configuration.InstanceLoadDelayTime, 1000, 5000);
            ImGui.Spacing();
        }

        private void DrawEnableClickThroughCheckbox()
        {
            ImGui.Checkbox("Enable Clickthrough", ref Service.Configuration.EnableClickthrough);
            ImGui.Spacing();
        }

        public override void Draw()
        {
            if(!IsOpen)
            {
                return;
            }
            else
            {
                DrawEnablePluginButton();
                DrawForceShowBannerCheckbox();
                DrawEnableClickThroughCheckbox();
                DrawInstanceLoadDelayTimeTextField();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            Service.Configuration.Save();
        }
    }
}
