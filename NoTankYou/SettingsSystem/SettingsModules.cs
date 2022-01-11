using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.SettingsSystem
{
    internal static class SettingsModules
    {
        public static readonly Dictionary<string, Configuration.ModuleSettings> Modules = new()
        {
            {"Dance Partner", Service.Configuration.DancePartnerSettings },
            {"Sage Kardion", Service.Configuration.KardionSettings },
            {"Scholar Faerie", Service.Configuration.FaerieSettings },
            {"Tank Stance", Service.Configuration.TankStanceSettings },
            {"Summoner Pet", Service.Configuration.SummonerSettings },
            {"Blue Mage Stance", Service.Configuration.BlueMageSettings },
            {"Food Check", Service.Configuration.FoodSettings }
        };
    }
}
