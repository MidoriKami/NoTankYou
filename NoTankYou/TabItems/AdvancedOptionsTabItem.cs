using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.TabItems
{
    internal class AdvancedOptionsTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.AdvancedOptions;

        public void DrawTabItem()
        {
            ImGui.Text( Strings.TabItems.AdvancedOptions.Label);
        }

        private static SystemSettings Settings => Service.Configuration.SystemSettings;

        private readonly InfoBox Contents = new()
        {
            Label = Strings.TabItems.AdvancedOptions.DisablePartyListChecking,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.TabItems.AdvancedOptions.DisablePartyListCheckingDisclaimer);
                ImGui.Spacing();
                ImGui.TextColored(Colors.SoftRed, Strings.TabItems.AdvancedOptions.StrongWarning);
                ImGui.Spacing();

                if (ImGui.Checkbox(Strings.TabItems.AdvancedOptions.DisableFeature, ref Settings.DisablePartyListVisibilityChecking))
                {
                    Service.Configuration.Save();
                }
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
