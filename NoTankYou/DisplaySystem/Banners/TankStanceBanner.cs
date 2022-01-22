using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class TankStanceBanner : WarningBanner
    {
        private readonly List<uint> TankStances = new();

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.TankStanceSettings;

        private const int TankRoleID = 1;

        public TankStanceBanner() : base("NoTankYou Warning Banner Window", "TankStance")
        {
            // Non-Blue Mage Tank Stances
            TankStances = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role == TankRoleID)
                .Select(r => r.StatusGainSelf.Value!)
                .Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();
        }

        protected override void UpdateInParty()
        {
            var tankPlayers = GetFilteredPartyList(p => p.ClassJob.GameData?.Role == TankRoleID);

            Visible = tankPlayers
                .All(p => !p.Statuses.Any(s => TankStances.Contains(s.StatusId))) && tankPlayers.Count > 0;
        }

        protected override void UpdateSolo()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsTank = player.ClassJob.GameData?.Role == TankRoleID && player.CurrentHp > 0;
            var tankStanceFound = player.StatusList.Any(s => TankStances.Contains(s.StatusId));

            Visible = playerIsTank && !tankStanceFound;
        }
    }
}
