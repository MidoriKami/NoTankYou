using Dalamud.Game.ClientState.Party;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;

namespace NoTankYou
{
    public class PartyOperations
    {
        private readonly List<string> TankStances = new() { "Iron Will", "Defiance", "Grit", "Royal Guard" };

        private List<ClassJob> tankClassJobs = new();

        public PartyOperations()
        {
            InitializeTankClassJobs();
        }

        public bool IsTank(ClassJob classJob)
        {
            return tankClassJobs.Contains(classJob);
        }

        private void InitializeTankClassJobs()
        {
            var classJobDatabase = Service.DataManager.GetExcelSheet<ClassJob>();

            // Role = 1, is "Tank" role
            tankClassJobs = classJobDatabase!.Where(r => r.Role == 1).ToList();
        }

        // Returns a list of all tanks, list will be empty if no tanks are found
        public List<PartyMember> GetTanksList()
        {
            var tankList = new List<PartyMember>();

            foreach (var player in Service.PartyList)
            {
                if (IsTank(player.ClassJob.GameData))
                {
                    tankList.Add(player);
                }
            }

            return tankList;
        }

        public bool IsTankStanceFound(PartyMember member)
        {
            foreach (var effect in member.Statuses)
            {
                if (IsTankStance(effect.GameData.Name))
                    return true;
            }

            return false;
        }

        public bool IsTankStance(string skillName)
        {
            return TankStances.Contains(skillName);
        }
    }
}
