using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.ClientState.Statuses;
using ImGuiScene;

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

        public FaerieBanner(TextureWrap faerieImage) : base("Partner Up Faerie Warning Banner", faerieImage)
        {

        }

        private bool DelayMilliseconds(int milliseconds, Stopwatch stopwatch)
        {
            if(stopwatch.IsRunning == false)
            {
                stopwatch.Start();
                return false;
            }

            if(stopwatch.ElapsedMilliseconds < milliseconds && stopwatch.IsRunning)
            {
                return false;
            }

            stopwatch.Stop();
            stopwatch.Reset();
            return true;
        }

        private IEnumerable<GameObject> GetPetsByOwnerId(uint ownerId)
        {
            return Service.ObjectTable
                .Where(o => o.OwnerId == ownerId)
                .Where(o => o.ObjectKind is ObjectKind.BattleNpc && ((BattleNpc)o).SubKind == (int)BattleNpcSubKind.Pet);
        }

        private Dictionary<PartyMember, Tuple<List<GameObject>, StatusList>> GetPartyMemberData()
        {
            Dictionary<PartyMember, Tuple<List<GameObject>, StatusList>> data = new();

            // Get Scholars
            var scholars = Service.PartyList.Where(p => p.ClassJob.Id is 28 && IsTargetable(p));

            foreach (var scholar in scholars)
            {
                // Get this scholars pets
                var playerPets = GetPetsByOwnerId(scholar.ObjectId).ToList();

                var statuses = scholar.Statuses;

                data.Add(scholar, new Tuple<List<GameObject>, StatusList>(playerPets, statuses));
            }

            return data;
        }

        protected override void UpdateInPartyInDuty()
        {
            var partyMemberData = GetPartyMemberData();

            var numPlayersWithPet = partyMemberData.Count(playerData => playerData.Value.Item1.Any());
            var numPlayersWithDissipation = partyMemberData.Count(playerData => playerData.Value.Item2.Any(s => s.StatusId is 791));

            var numPetsChanged = numPlayersWithPet != LastNumPlayersWithPets;
            var numDissipationChanged = numPlayersWithDissipation != LastNumPlayersWithDissipation;

            if (numPetsChanged)
            {
                if (DelayMilliseconds(500, PetCountStopwatch))
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
                if (DelayMilliseconds(500, DissipationCountStopwatch))
                {
                    LastNumPlayersWithDissipation = numPlayersWithDissipation;
                }
                else
                {
                    return;
                }
            }

            Visible = partyMemberData.
                Where(player => !player.Value.Item1.Any()).// Where the player doesn't have any pets
                Any(player => !player.Value.Item2.Any(s => s.StatusId is 791)); // If any of them don't have dissipation
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
                if (DelayMilliseconds(500, SoloPetStopwatch))
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
                if (DelayMilliseconds(500, SoloDissipationStopwatch))
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
