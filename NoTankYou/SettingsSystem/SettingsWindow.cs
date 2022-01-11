using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.SettingsSystem.SettingsCategories;
using System.Numerics;

namespace NoTankYou.SettingsSystem
{
    internal class SettingsWindow : Window
    {
        private Tab CurrentTab = Tab.General;
        private readonly Vector2 WindowSize = new(500, 500);

        private readonly Dictionary<Tab, TabCategory> SettingsCategories = new()
        {
            { Tab.General, new GeneralSettings() },
            { Tab.Display, new DisplaySettings() },
            { Tab.TankStance, new TankStanceSettings() },
            { Tab.DancePartner, new DancePartnerSettings() },
            { Tab.Faerie, new FaerieSettings() },
            { Tab.Kardion, new KardionSettings() },
            { Tab.Summoner, new SummonerPetSettings() },
            { Tab.BlueMage, new BlueMageSettings() },
            { Tab.Food, new FoodSettings()},
            { Tab.Blacklist, new BlacklistSettings() }
        };
    
        private enum Tab
        {
            General,
            Display,
            TankStance,
            DancePartner,
            Faerie,
            Kardion,
            Summoner,
            BlueMage,
            Food,
            Blacklist
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

            DrawTabs();

            SettingsCategories[CurrentTab].Draw();

            ImGui.Separator();
            DrawSaveAndCloseButtons();
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
            ImGui.SetCursorPos(new Vector2(5, windowSize.Y - 30));

            if (ImGui.Button("Save", new(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 155);

            if (ImGui.Button("Save & Close", new(150, 25)))
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
