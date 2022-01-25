using NoTankYou.SettingsSystem.DisplayModules.SettingsTab.SubModules;

namespace NoTankYou.SettingsSystem.DisplayModules.SettingsTab
{
    internal class GeneralSettings : DisplayModule
    {

        private readonly GeneralBasicSettings GeneralBasicSettings = new();

        public GeneralSettings()
        {
            CategoryString = "General Settings";
        }

        protected override void DrawContents()
        {
            GeneralBasicSettings.Draw();
        }

        public override void Dispose()
        {
            
        }
    }
}
