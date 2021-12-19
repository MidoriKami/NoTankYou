using System.Linq;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class KardionBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeKardionBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowKardionBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableKardionBannerWhileSolo;

        public KardionBanner() : base("Partner Up Kardion Warning Banner", "Kardion")
        {
        }

        protected override void UpdateInPartyInDuty()
        {
            Visible = !Service.PartyList
                .Where(r => r.ClassJob.Id is 40)
                .Any(p => p.Statuses.Any(s => s.StatusId is 2604));
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            bool playerIsSage = player.ClassJob.Id == 40;

            var playerHasKardia = player.StatusList.Any(s => s.StatusId is 2604);

            Visible = playerIsSage && !playerHasKardia;
        }
    }
}
