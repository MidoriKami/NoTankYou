using System.Linq;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class DancePartnerBanner : WarningBanner
    {
        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.DancePartnerSettings;

        private const int DancerClassId = 38;
        private const int ClosedPositionStatusId = 1823;

        public DancePartnerBanner() : base("Partner Up Dance Partner Warning Banner", "DancePartner")
        {

        }


        protected override void UpdateInParty()
        {
            var dancerPlayers = GetFilteredPartyList(p => p.ClassJob.Id == DancerClassId && p.Level >= 60);

            Visible = dancerPlayers
                .Any(p => !p.Statuses.Any(s => s.StatusId is ClosedPositionStatusId));
        }

        protected override void UpdateSolo()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsDancer = player.ClassJob.Id == DancerClassId && player.Level >= 60 && player.CurrentHp > 0;

            var playerHadClosedPosition = player.StatusList.Any(s => s.StatusId is ClosedPositionStatusId);

            Visible = playerIsDancer && !playerHadClosedPosition;
        }
    }
}
