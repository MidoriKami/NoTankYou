using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class BlueMageTankStance : WarningBanner
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.BlueMageSettings;

        private const int BlueMageClassJobID = 36;
        private const int MightyGuardStatusID = 1719;
        private const int AetherialMimicryTankStatusID = 2124;

        public BlueMageTankStance() : base("No Tank You Blue Mage Tank Stance Warning Banner", "BlueMage")
        {

        }

        protected override void UpdateInParty()
        {
            var blueMagePlayers = GetFilteredPartyList(p => p.ClassJob.Id == BlueMageClassJobID);

            var blueMageTanks = blueMagePlayers.Where(p => p.Statuses.Any(s => s.StatusId == AetherialMimicryTankStatusID));

            Visible = blueMageTanks
                .All(p => p.Statuses.All(s => s.StatusId != MightyGuardStatusID));
        }

        protected override void UpdateSolo()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsAlive = player.CurrentHp > 0;

            var playerIsBlueMage = player.ClassJob.Id == BlueMageClassJobID;

            var playerIsTank = true; //player.StatusList.Any(s => s.StatusId == AetherialMimicryTankStatusID);

            var tankStanceFound = player.StatusList.Any(s => s.StatusId == MightyGuardStatusID);

            Visible = playerIsAlive && playerIsBlueMage && playerIsTank && !tankStanceFound;
        }
    }
}
