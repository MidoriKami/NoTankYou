using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

            this.SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize;
        }

        public void Dispose()
        {
            
        }

        private void DrawForceShowWindowButton()
        {
            if (Service.Configuration.ForceShowNoTankWarning == true)
            {
                if (ImGui.Button("Disable Force Show Warning"))
                {
                    Service.Configuration.ForceShowNoTankWarning = false;
                }
            }
            else
            {
                if (ImGui.Button("Enable Force Show Warning"))
                {
                    Service.Configuration.ForceShowNoTankWarning = true;
                }
            }
        }

        private void DrawEnablePluginButton()
        {
            if (Service.Configuration.ShowNoTankWarning == true)
            {
                if (ImGui.Button("Disable No Tank Stance Warning"))
                {
                    Service.Configuration.ShowNoTankWarning = false;
                }
            }
            else
            {
                if (ImGui.Button("Enable No Tank Stance Warning"))
                {
                    Service.Configuration.ShowNoTankWarning = true;
                }
            }
        }

        private void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Hide Warning for x milliseconds on mapload");
            ImGui.InputInt("", ref Service.Configuration.InstanceLoadDelayTime, 1000, 5000);
        }
        private void DrawDisableInAllianceRaidsCheckbox()
        {
            ImGui.Checkbox("Disable in Alliance Raids", ref Service.Configuration.DisableInAllianceRaids);
        }

        private void DrawEnableClickThroughCheckbox()
        {
            ImGui.Checkbox("Disable Clickthrough", ref Service.Configuration.DisableClickthrough);
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
                DrawForceShowWindowButton();
                DrawEnableClickThroughCheckbox();
                DrawInstanceLoadDelayTimeTextField();
                DrawDisableInAllianceRaidsCheckbox();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            Service.Configuration.Save();
        }
    }
}
