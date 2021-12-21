using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class TankStanceBanner : WarningBanner
    {
        private readonly List<uint> TankStances = new();

        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeTankStanceBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowTankStanceBanner;
        protected override ref bool ModuleEnabled => ref Service.Configuration.EnableTankStanceBanner;


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

        protected override void UpdateInPartyInDuty()
        {
            ICollection<PartyMember> tankPlayers = Service.PartyList
                .Where(p => p.ClassJob.GameData.Role == TankRoleID).ToArray();

            FilterDeadPlayers(ref tankPlayers);

            Visible = tankPlayers
                .Any(p => !p.Statuses.Any(s => TankStances.Contains(s.StatusId)));
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsTank = player.ClassJob.GameData.Role == TankRoleID;
            var tankStanceFound = player.StatusList.Any(s => TankStances.Contains(s.StatusId));

            Visible = playerIsTank && !tankStanceFound;
        }

        protected override void UpdateSoloEverywhere()
        {
            UpdateSoloInDuty();
        }
    }
}
