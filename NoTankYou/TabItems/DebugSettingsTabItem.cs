using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data;
using NoTankYou.Enums;
using NoTankYou.Interfaces;

namespace NoTankYou.TabItems
{
    internal class DebugSettingsTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.Debugging;

        private static DebugSettings Settings => Service.Configuration.DebugSettings;

        public void DrawTabItem()
        {
            ImGui.Text("Debugging");
        }

        public void DrawConfigurationPane()
        {
            ImGuiHelpers.ScaledDummy(10.0f);

            new InfoBox
            {
                Label = "Log Channels",
                ContentsAction = () =>
                {
                    if (ImGui.Checkbox(LogChannel.ContentDirector.ToString(), ref Settings.ContentDirector))
                    {
                        Service.Configuration.Save();
                    }
                }
            }.DrawCentered();
        }
    }
}
