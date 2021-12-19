using ImGuiScene;
using System.Linq;

namespace NoTankYou.DisplaySystem
{
    internal class DancePartnerBanner : WarningBanner
    {
        private bool DancePartnerSoloMode = false;
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeDancePartnerBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowDancePartnerBanner;
        protected override ref bool SoloModeBool => ref DancePartnerSoloMode;

        public DancePartnerBanner(TextureWrap dancePartnerImage) : base("Partner Up Dance Partner Warning Banner", dancePartnerImage)
        {

        }

        protected override void UpdateInPartyInDuty()
        {
            // ClassJob 38 is Dancer, get the list of all dancers in the party
            var partyList = Service.PartyList.Where(r => r.ClassJob.Id is 38 && r.Level >= 60);

            // Closed Position Status is 1823, get the list of these dancers with Closed Position
            var membersWithClosedPosition = partyList.Where(r => r.Statuses.Any(s => s.StatusId == 1823) && r.Level >= 60);

            // If these two lists match, then everyone's doing their job
            if (partyList.Count() == membersWithClosedPosition.Count())
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
            // Dance Partner Does not Support Solo Mode
            return;
        }
    }
}
