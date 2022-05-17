using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class FoodConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Food;
        public string ConfigurationPaneLabel => Strings.Modules.Food.ConfigurationPanelLabel;
        public InfoBox? AboutInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }

        private static FoodModuleSettings Settings => Service.Configuration.ModuleSettings.Food;

        public FoodConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Food.Label);
        }

        public readonly InfoBox Options = new()
        {
            Label = Strings.Common.Labels.Options,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Configuration.Enable, ref Settings.Enabled))
                {
                    Service.Configuration.Save();
                }
            }
        };

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);
        }

    }
}
