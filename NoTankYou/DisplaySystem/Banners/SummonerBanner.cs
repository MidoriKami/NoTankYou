using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using NoTankYou.Utilities;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class SummonerBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeSummonerBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowSummonerBanner;
        protected override ref bool ModuleEnabled => ref Service.Configuration.EnableSummonerBanner;

        private readonly Stopwatch PetCountStopwatch = new();
        private readonly Stopwatch SoloPetStopwatch = new();

        private const int SummonerClassID = 27;

        private int LastNumPlayersWithPets = 0;
        private bool LastSoloPet = false;

        public SummonerBanner() : base("No Tank You Summoner Pet Warning Banner", "Summoner")
        {
        }



        protected override void UpdateInPartyInDuty()
        {
            var partyMemberData = PetUtilities.GetPartyMemberData(SummonerClassID);

            var numPlayersWithPet = partyMemberData.Count(playerData => playerData.Value.Item1.Any());

            var numPetsChanged = numPlayersWithPet != LastNumPlayersWithPets;

            if (numPetsChanged)
            {
                if (PetUtilities.DelayMilliseconds(500, PetCountStopwatch))
                {
                    LastNumPlayersWithPets = numPlayersWithPet;
                }
                else
                {
                    return;
                }
            }

            Visible = partyMemberData // Where the player doesn't have any pets
                .Any(player => !player.Value.Item1.Any());
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsScholar = player.ClassJob.Id == SummonerClassID;

            var isPetPresent = Service.BuddyList.PetBuddyPresent;


            var petStatusChanged = isPetPresent != LastSoloPet;

            if (petStatusChanged)
            {
                if (PetUtilities.DelayMilliseconds(500, SoloPetStopwatch))
                {
                    LastSoloPet = isPetPresent;
                }
                else
                {
                    return;
                }
            }

            Visible = playerIsScholar && !isPetPresent;
        }

        protected override void UpdateSoloEverywhere()
        {
            UpdateSoloInDuty();
        }
    }
}
