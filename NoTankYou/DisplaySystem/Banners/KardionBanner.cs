using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class KardionBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeKardionBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowKardionBanner;
        protected override ref bool ModuleEnabled => ref Service.Configuration.EnableKardionBanner;

        private const int SageClassID = 40;
        private const int KardiaStatusID = 2604;

        public KardionBanner() : base("No Tank You Kardion Warning Banner", "Kardion")
        {

        }

        protected override void UpdateInParty()
        {
            var sagePlayers = Service.PartyList
                .Where(r => r.ClassJob.Id == SageClassID).ToList();

            var deadPlayers = GetDeadPlayers(sagePlayers);

            sagePlayers.RemoveAll(r => deadPlayers.Contains(r.ObjectId));

            Visible = sagePlayers
                .Any(p => p.Statuses.All(s => s.StatusId != KardiaStatusID));
        }

        protected override void UpdateSolo()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsSage = player.ClassJob.Id == SageClassID;

            var playerHasKardia = player.StatusList.Any(s => s.StatusId == KardiaStatusID);

            Visible = playerIsSage && !playerHasKardia;
        }
    }
}
