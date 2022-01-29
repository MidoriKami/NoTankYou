using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules
{
    internal class GeneralBasicSettings : SettingsCategory
    {
        public GeneralBasicSettings() : base("Basic Configuration Settings")
        {
        }

        protected override void DrawContents()
        {
            DrawDisableInAllianceRaids();

            DrawChangeWaitFrameCount();

            DrawInstanceLoadDelayTimeTextField();

            DrawDeathGracePeriod();
        }

        private static void DrawDisableInAllianceRaids()
        {
            ImGui.Checkbox("Disable in Alliance Raids", ref Service.Configuration.DisableInAllianceRaid);

            ImGui.Spacing();
        }

        private static void DrawChangeWaitFrameCount()
        {
            ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("Number of Wait Frames", ref Service.Configuration.NumberOfWaitFrames, 0, 0);
            ImGui.PopItemWidth();
            ImGuiComponents.HelpMarker("How many frames to wait between warning evaluations.\n" +
                                       "Higher values represents a larger delay on updating warnings.\n" +
                                       "Recommend half your displays refresh rate.\n" +
                                       "Minimum: 1\n" +
                                       "Maximum: 144");

            if (Service.Configuration.NumberOfWaitFrames < 1)
            {
                Service.Configuration.NumberOfWaitFrames = 1;
            }

            if (Service.Configuration.NumberOfWaitFrames > 144)
            {
                Service.Configuration.NumberOfWaitFrames = 144;
            }

            ImGui.Spacing();
        }

        private static void DrawInstanceLoadDelayTimeTextField()
        {
            ImGui.Text("Instance Load Grace Period");
            
            ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("##TerritoryChangeDelay", ref Service.Configuration.TerritoryChangeDelayTime, 1000, 5000);
            ImGui.PopItemWidth();

            ImGuiComponents.HelpMarker("Hide warnings on map change for (milliseconds)\n" +
                                       "Recommended: 8,000 - 15,000\n" +
                                       "Minimum: 3,000");

            if (Service.Configuration.TerritoryChangeDelayTime < 3000)
            {
                Service.Configuration.TerritoryChangeDelayTime = 3000;
            }

            ImGui.Spacing();
        }

        private void DrawDeathGracePeriod()
        {
            ImGui.Text("Death Grace Period");

            ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("##DeathChangeDelay", ref Service.Configuration.DeathGracePeriod, 1000, 5000);
            ImGui.PopItemWidth();
            
            ImGuiComponents.HelpMarker("Wait this many milliseconds before warning about someone who just rezed from death\n" +
                                       "Recommended: 5,000\n" +
                                       "Minimum: 1,000");

            if (Service.Configuration.DeathGracePeriod < 1000)
            {
                Service.Configuration.DeathGracePeriod = 1000;
            }

            ImGui.Spacing();
        }
    }
}
