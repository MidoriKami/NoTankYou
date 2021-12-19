using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class TankStanceBanner : WarningBanner
    {
        private readonly List<uint> TankStances = new();

        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeTankStanceBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowTankStanceBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableTankStanceBannerWhileSolo;

        public TankStanceBanner() : base("NoTankYou Warning Banner Window", "TankStance")
        {
            // Non-Blue Mage Tank Stances
            TankStances = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role is 1)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();
        }

        protected override void UpdateInPartyInDuty()
        {
            Visible = !Service.PartyList
                .Where(p => p.ClassJob.GameData.Role is 1)
                .Any(p => p.Statuses.Any(s => TankStances.Contains(s.StatusId)));
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsTank = player.ClassJob.GameData.Role == 1;
            var tankStanceFound = player.StatusList.Any(s => TankStances.Contains(s.StatusId));

            Visible = playerIsTank && !tankStanceFound;
        }
    }
}
