using System.Collections.Generic;
using System.Linq;
using NoTankYou.Interfaces;
using NoTankYou.Modules;

namespace NoTankYou.System
{
    public class ModuleManager
    {
        private List<IModule> Modules { get; } = new()
        {
            new TankModule(),
            new DancerModule(),
            new ScholarModule(),
            new SageModule(),
            new SummonerModule(),
            new FoodModule(),
            new FreeCompanyModule(),
            new BlueMageModule(),
            new SpiritbondModule(),
        };

        public void ProcessCommand(string command, string arguments)
        {
            foreach (var module in Modules.OfType<ICommand>())
            {
                module.ProcessCommand(command, arguments);
            }
        }

        public List<IModule> GetModulesForClassJob(uint job)
        {
            return Modules
                .Where(module => module.ClassJobs.Contains(job))
                .ToList();
        }
    }
}
