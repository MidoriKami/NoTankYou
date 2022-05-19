using System.Collections.Generic;
using System.Linq;
using NoTankYou.Interfaces;
using NoTankYou.Modules;

namespace NoTankYou.System
{
    public class ModuleManager
    {
        public List<IModule> Modules { get; set; } = new()
        {
            new TankModule(),
            new DancerModule(),
            new ScholarModule(),
            new SageModule(),
            new SummonerModule(),
            new FoodModule(),
        };

        public List<IModule> GetModulesForClassJob(uint job)
        {
            return Modules
                .Where(module => module.ClassJobs.Contains(job))
                .ToList();
        }
    }
}
