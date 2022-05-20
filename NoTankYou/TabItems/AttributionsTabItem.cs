using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.TabItems
{
    internal class AttributionsTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.Attributions;

        public void DrawTabItem()
        {
            ImGui.Text(Strings.TabItems.Attributions.Label);
        }

        private readonly InfoBox Contents = new()
        {
            Label = Strings.TabItems.Attributions.Label,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.TabItems.Attributions.FreeCompanyDescription);

                ImGui.Spacing();
            }
        };

        public void DrawConfigurationPane()
        {
            ImGuiHelpers.ScaledDummy(20.0f);
            Contents.DrawCentered(0.90f);

            ImGuiHelpers.ScaledDummy(30.0f);
        }
    }
}
