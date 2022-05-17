using NoTankYou.Data.Modules;

namespace NoTankYou.Data
{
    public class ModuleSettings
    {
        public DancerModuleSettings Dancer { get; set; } = new();
        public FoodModuleSettings Food { get; set; } = new();
        public SageModuleSettings Sage { get; set; } = new();
        public ScholarModuleSettings Scholar { get; set; } = new();
        public SummonerModuleSettings Summoner { get; set; } = new();
        public TankModuleSettings Tank { get; set; } = new();
        public BlueMageModuleSettings BlueMage { get; set; } = new();
    }
}
