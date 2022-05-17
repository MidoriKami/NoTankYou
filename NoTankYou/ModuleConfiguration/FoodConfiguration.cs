using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using NoTankYou.Components;
using NoTankYou.Data.Components;
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
        public string AboutInformationBox => Strings.Modules.Food.Description;
        public string TechnicalInformation => Strings.Modules.Food.TechnicalDescription;
        public GenericSettings GenericSettings => Settings;
        public TextureWrap? AboutImage { get; }
        private static FoodModuleSettings Settings => Service.Configuration.ModuleSettings.Food;

        public FoodConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Food.Label);
        }

        public void DrawOptions()
        {
        }
    }
}
