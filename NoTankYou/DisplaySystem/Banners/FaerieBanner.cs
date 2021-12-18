using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Buddy;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using ObjectKind = Dalamud.Game.ClientState.Objects.Enums.ObjectKind;
using NoTankYou.Utilities;

namespace NoTankYou.DisplaySystem
{
    internal class FaerieBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeFaerieBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowFaerieBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableFaerieBannerWhileSolo;

        private readonly StateDebouncer<bool> StateDebouncer = new(20);
        private readonly StateDebouncer<int> PartyPetStateDebouncer = new(20);
        public FaerieBanner(TextureWrap faerieImage) : base("Partner Up Faerie Warning Banner", faerieImage)
        {

        }

        protected override void UpdateInPartyInDuty()
        {
            // Scholar Job id is 28
            var scholarPlayers = Service.PartyList.Where(p => p.ClassJob.Id is 28).ToHashSet();

            // If they are untargetable, remove them from the count
            foreach (var player in scholarPlayers)
            {
                if (!IsTargetable(player))
                {
                    scholarPlayers.Remove(player);
                }
            }

            var scholarPlayerIDs = scholarPlayers.Select(r => r.ObjectId);

            // Get the pet objects that have owner ids matching those of our scholars
            var petObjectsOwnedByScholarPartyMember = Service.ObjectTable
                .Where(r => scholarPlayerIDs.Contains(r.OwnerId))
                .Where(r => r.ObjectKind is ObjectKind.BattleNpc)
                .Where(r => (r as BattleNpc)!.BattleNpcKind == BattleNpcSubKind.Pet);

            PartyPetStateDebouncer.AddState(petObjectsOwnedByScholarPartyMember.Count());

            // id 791 is dissipation id
            var dissipationEffects = scholarPlayers
                .Where(r => r.Statuses.Any(s => s.StatusId is 791));

            // Due to debouncing, there could be more faeries + dissipations, than sholars
            if ( PartyPetStateDebouncer.EvaluateState() + dissipationEffects.Count() >= scholarPlayers.Count )
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            }
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            // If the player isn't a Scholar return
            if (player.ClassJob.Id != 28) return;

            var isPetPresent = Service.BuddyList.PetBuddyPresent;

            StateDebouncer.AddState(isPetPresent);

            // id 791 is dissipation id
            bool playerHasDissipation = player.StatusList.Any(s => s.StatusId is 791);

            // If player has dissipation, or a faerie out, they are doing their job
            if (playerHasDissipation || StateDebouncer.EvaluateState())
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            }
        }
    }
}
