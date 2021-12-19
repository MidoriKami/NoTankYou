using System.Collections.Generic;
using System.Linq;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.DisplaySystem
{
    internal class TankStanceBanner : WarningBanner
    {
        private readonly List<uint> TankStances = new();
        private readonly List<uint> BlueMageTankStance = new();

        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeTankStanceBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowTankStanceBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableTankStanceBannerWhileSolo;

        public TankStanceBanner(ImGuiScene.TextureWrap warningImage) :
            base("NoTankYou Warning Banner Window", warningImage)
        {
            // Non-Blue Mage Tank Stances
            TankStances = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role is 1)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();

            // Blue Mage Tank Stances
            BlueMageTankStance = Service.DataManager.GetExcelSheet<Action>()
                !.Where(r => r.ClassJob.Value?.Role is 3)
                !.Select(r => r.StatusGainSelf.Value!)
                !.Where(r => r.IsPermanent == true)
                .Select(r => r.RowId)
                .ToList();
        }

        protected override void UpdateInPartyInDuty()
        {
            // Get all the Tanks
            var tanks = Service.PartyList.Where(r => r.ClassJob.GameData.Role is 1);
            var blueMageTanks = Service.PartyList.Where(r => r.ClassJob.Id is 36 && r.Statuses.Any(s => s.StatusId == 2124));

            // Get the Tanks that have a tank stance on
            var tankStances = tanks.Where(r => r.Statuses.Any(s => TankStances.Contains(s.StatusId)));
            var blueMageTankStances = blueMageTanks.Where(r => r.Statuses.Any(s => BlueMageTankStance.Contains(s.StatusId)));

            var totalNumberOfTanks = tanks.Count() + blueMageTanks.Count();
            var totalNumberOfTankStances = tankStances.Count() + blueMageTankStances.Count();

            bool atLeastOneTankInParty = totalNumberOfTanks > 0;
            bool noTankStancesFound = totalNumberOfTankStances == 0;

            if (noTankStancesFound && atLeastOneTankInParty)
            {
                Visible = true;
            }
            else
            {
                Visible = false;
            }
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            var playerIsRegularTank = player.ClassJob.GameData.Role == 1;

            // Only Blue Mages with Mimicry Tank are counted as tanks
            var playerIsBlueMage = player.ClassJob.Id == 36;
            var blueMageMightyGuard = player.StatusList.Any(s => s.StatusId is 2124);

            bool playerIsBlueTank = playerIsBlueMage && blueMageMightyGuard;
            bool playerIsTank = playerIsRegularTank || playerIsBlueTank;

            var tankStanceFound = player.StatusList.Any(s => TankStances.Contains(s.StatusId));
            var blueMageStanceFound = player.StatusList.Any(s => BlueMageTankStance.Contains(s.StatusId));

            if (tankStanceFound || blueMageStanceFound)
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            }

            return;
        }
    }
}
