namespace NoTankYou.SettingsSystem.DisplayModules.WarningsTab.SubModules
{
    internal class TankStancesSettingsModule : DisplayModule
    {
        private readonly TankStanceSettings TankStancesSettings = new();
        private readonly BlueMageSettings BlueMageSettings = new();

        public TankStancesSettingsModule()
        {
            CategoryString = "Tank Stances";
        }

        protected override void DrawContents()
        {
            TankStancesSettings.Draw();

            BlueMageSettings.Draw();
        }

        public override void Dispose()
        {
        }
    }
}
