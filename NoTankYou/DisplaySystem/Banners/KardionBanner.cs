using System.Linq;
using ImGuiScene;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class KardionBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeKardionBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowKardionBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableKardionBannerWhileSolo;

        public KardionBanner(TextureWrap kardionImage) : base("Partner Up Kardion Warning Banner", kardionImage)
        {
        }

        protected override void UpdateInPartyInDuty()
        {
            // ClassJob 40 is Sage, get the list of all Sages in the party
            var partyList = Service.PartyList.Where(r => r.ClassJob.Id is 40);

            // Kardion Status is 2605, get the list of party members with Kardion
            // Kardia Status is 2604 (This goes on the sage)
            var membersWithKardion = Service.PartyList.Where(r => r.Statuses.Any(s => s.StatusId == 2604));

            // If these two lists match, then everyone's doing their job
            if (partyList.Count() == membersWithKardion.Count())
            {
                Visible = false;
            }

            // If not, then we need to show the warning banner
            else
            {
                Visible = true;
            }
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;

            if (player == null) return;

            // If the player isn't a sage, then this is irrelevent
            if (player.ClassJob.Id != 40) return;

            // Does player have Kardia
            var playerHasKardia = player.StatusList.Any(s => s.StatusId is 2604);

            // If the player has Kardia on themselves, they are doing their job
            if (playerHasKardia)
            {
                Visible = false;
            }
            // If not, then we need to show the warning banner
            else
            {
                Visible = true;
            }
        }
    }
}
