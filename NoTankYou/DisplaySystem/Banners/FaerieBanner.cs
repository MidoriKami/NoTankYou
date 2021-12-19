using System.Diagnostics;
using System.Linq;
using NoTankYou.Utilities;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class FaerieBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeFaerieBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowFaerieBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableFaerieBannerWhileSolo;

        private readonly Stopwatch PetCountStopwatch = new();
        private readonly Stopwatch DissipationCountStopwatch = new();
        private readonly Stopwatch SoloPetStopwatch = new();
        private readonly Stopwatch SoloDissipationStopwatch = new();
        private int LastNumPlayersWithPets = 0;
        private int LastNumPlayersWithDissipation = 0;
        private bool LastSoloPet = false;
        private bool LastSoloDissipation = false;

        public FaerieBanner() : base("Partner Up Faerie Warning Banner", "Faerie")
        {

        }

        protected override void UpdateInPartyInDuty()
        {
            var partyMemberData = PetUtilities.GetPartyMemberData();

            var numPlayersWithPet = partyMemberData.Count(playerData => playerData.Value.Item1.Any());
            var numPlayersWithDissipation = partyMemberData.Count(playerData => playerData.Value.Item2.Any(s => s.StatusId is 791));

            var numPetsChanged = numPlayersWithPet != LastNumPlayersWithPets;
            var numDissipationChanged = numPlayersWithDissipation != LastNumPlayersWithDissipation;

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

            if (numDissipationChanged)
            {
                if (PetUtilities.DelayMilliseconds(500, DissipationCountStopwatch))
                {
                    LastNumPlayersWithDissipation = numPlayersWithDissipation;
                }
                else
                {
                    return;
                }
            }

            Visible = partyMemberData
                .Where(player => !player.Value.Item1.Any())// Where the player doesn't have any pets
                .Any(player => !player.Value.Item2.Any(s => s.StatusId is 791)); // If any of them don't have dissipation
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            // If the player isn't a Scholar return
            if (player.ClassJob.Id != 28) return;

            var isPetPresent = Service.BuddyList.PetBuddyPresent;

            // id 791 is dissipation id
            var playerHasDissipation = player.StatusList.Any(s => s.StatusId is 791);

            var petStatusChanged = isPetPresent != LastSoloPet;
            var dissipationStatusChange = playerHasDissipation != LastSoloDissipation;

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

            if (dissipationStatusChange)
            {
                if (PetUtilities.DelayMilliseconds(500, SoloDissipationStopwatch))
                {
                    LastSoloDissipation = playerHasDissipation;
                }
                else
                {
                    return;
                }
            }

            Visible = !(playerHasDissipation || isPetPresent);
        }
    }
}
