using System.IO;
using System.Linq;
using ImGuiScene;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class DancePartnerBanner : WarningBanner
    {
        private bool DancePartnerSoloMode = false;
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeDancePartnerBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowDancePartnerBanner;
        protected override ref bool SoloModeBool => ref DancePartnerSoloMode;

        public DancePartnerBanner() : base("Partner Up Dance Partner Warning Banner", "DancePartner")
        {

        }

        protected override void UpdateInPartyInDuty()
        {
            Visible = Service.PartyList
                .Where(p => p.ClassJob.Id is 38 && p.Level >= 60)
                .Any(p => !p.Statuses.Any(s => s.StatusId is 1823));
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if(player == null) return;

            var playerIsDancer = player.ClassJob.Id == 38;

            var playerHadClosedPosition = player.StatusList.Any(s => s.StatusId is 1823);

            Visible = playerIsDancer && !playerHadClosedPosition;
        }
    }
}
