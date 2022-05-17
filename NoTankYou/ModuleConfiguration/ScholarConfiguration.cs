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
    internal class ScholarConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Scholar;
        public string ConfigurationPaneLabel => Strings.Modules.Scholar.ConfigurationPanelLabel;
        public string AboutInformationBox => Strings.Modules.Scholar.Description;
        public string TechnicalInformation => Strings.Modules.Scholar.TechnicalDescription;
        public TextureWrap? AboutImage { get; }
        public GenericSettings GenericSettings => Settings;
        private static ScholarModuleSettings Settings => Service.Configuration.ModuleSettings.Scholar;

        public ScholarConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Scholar.Label);
        }

        public void DrawOptions()
        {

        }
    }
}
