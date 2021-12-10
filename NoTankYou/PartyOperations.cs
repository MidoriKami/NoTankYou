using Dalamud.Game.ClientState.Party;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;

namespace NoTankYou
{
    public class PartyOperations
    {
        private List<ClassJob> tankClassJobs = new();
        private List<Status> tankStatus = new();

        public PartyOperations()
        {
            InitializeTankClassJobs();
            InitializeTankStances();
        }

        private void InitializeTankStances()
        {            
            var actionSheet = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>();

            // Role 1 = tank, Role 3 = Caster DPS (For BLU)
            tankStatus = actionSheet
                !.Where(r => r.ClassJob.Value?.Role is 1 or 3)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .ToList();

        }
        public bool IsTank(PartyMember member)
        {
            if(tankClassJobs.Contains(member.ClassJob.GameData))
            {
                return true;
            }

            const uint blueMageClassID = 36;
            if(member.ClassJob.Id == blueMageClassID)
            {
                foreach ( var status in member.Statuses)
                {
                    const uint aethericalMimicryTank = 2124;
                    if (status.StatusId == aethericalMimicryTank)
                    {
                        return true;
                    }
                }
            }

            return false;
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
                if ( IsTank(player) )
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
                if (IsTankStance(effect.GameData))
                    return true;
            }

            return false;
        }

        public bool IsTankStance(Status skillName)
        {
            return tankStatus.Contains(skillName);
        }
    }
}
