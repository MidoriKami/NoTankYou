using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface;
using NoTankYou.SettingsSystem.SettingsTabs;

namespace NoTankYou.SettingsSystem
{
    internal class SettingsWindow : Window
    {
        private Tab CurrentTab = Tab.Settings;
        private readonly Vector2 WindowSize = new(550, 650);


        private readonly Dictionary<Tab, TabCategory> SettingsCategories = new()
        {
            {Tab.Settings, new SettingsTab() },
            {Tab.Warnings, new WarningsTab() }
        };
    
        private enum Tab
        {
            Settings,
            Warnings
        }

        public SettingsWindow() : base("No Tank You Settings")
        {
            IsOpen = false;

            SizeConstraints = new Window.WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Flags |= ImGuiWindowFlags.NoResize;
            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(10, 5));

            DrawTabs();

            SettingsCategories[CurrentTab].Draw();

            ImGui.Separator();
            DrawSaveAndCloseButtons();

            ImGui.PopStyleVar();
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("No Tank You Settings", ImGuiTabBarFlags.NoTooltip))
            {
                foreach (var (tab, data) in SettingsCategories)
                {
                    if (ImGui.BeginTabItem(data.TabName))
                    {
                        CurrentTab = tab;
                        ImGui.EndTabItem();
                    }
                }
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            Service.Configuration.Save();
        }

        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            var windowSize = ImGui.GetWindowSize();
            ImGui.SetCursorPos(new Vector2(5, windowSize.Y - 30 * ImGuiHelpers.GlobalScale));

            if (ImGui.Button("Save", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 155 *ImGuiHelpers.GlobalScale);

            if (ImGui.Button("Save & Close", ImGuiHelpers.ScaledVector2(150, 25)))
            {
                Service.Configuration.Save();
                IsOpen = false;
            }

            ImGui.Spacing();
        }

        public void Dispose()
        {

        }
    }
}
