using NoTankYou.Utilities;
using System.Diagnostics;
using System.Linq;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class SummonerBanner : WarningBanner
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.SummonerSettings;


        private readonly Stopwatch PetCountStopwatch = new();
        private readonly Stopwatch SoloPetStopwatch = new();

        private const int SummonerClassID = 27;

        private int LastNumPlayersWithPets = 0;
        private bool LastSoloPet = false;

        public SummonerBanner() : base("No Tank You Summoner Pet Warning Banner", "Summoner")
        {
        }



        protected override void UpdateInParty()
        {
            var partyMembers = GetFilteredPartyList(p => p.ClassJob.Id == SummonerClassID && IsTargetable(p));

            var partyMemberData = PetUtilities.GetPartyMemberData(partyMembers);

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

        protected override void UpdateSolo()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsScholar = player.ClassJob.Id == SummonerClassID && player.CurrentHp > 0;

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
    }
}
