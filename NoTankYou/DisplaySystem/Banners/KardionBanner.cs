using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class KardionBanner : WarningBanner
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.KardionSettings;


        private const int SageClassID = 40;
        private const int KardiaStatusID = 2604;

        public KardionBanner() : base("No Tank You Kardion Warning Banner", "Kardion")
        {

        }

        protected override void UpdateInParty()
        {
            var sagePlayers = GetFilteredPartyList(p => p.ClassJob.Id == SageClassID);

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
