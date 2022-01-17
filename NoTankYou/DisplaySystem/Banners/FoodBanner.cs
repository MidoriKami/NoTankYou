using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;

namespace NoTankYou.DisplaySystem.Banners
{
    internal class FoodBanner : WarningBanner
    {
        private const int WellFedStatusID = 48;

        public FoodBanner() : base("NoTankYou Food Warning Banner", "Food")
        {
        }

        protected override ref Configuration.ModuleSettings Settings => ref Service.Configuration.FoodSettings;

        protected override void UpdateInParty()
        {
            if (Service.Configuration.FoodSoloMode == true)
            {
                UpdateSolo();
                return;
            }

            var currentTerritory = Service.ClientState.TerritoryType;

            bool currentlyInCombat = Service.Condition[ConditionFlag.InCombat];
            bool whiteListedTerritory = Service.Configuration.FoodTerritoryWhitelist.Contains(currentTerritory);

            if ( currentlyInCombat || !whiteListedTerritory )
            {
                Visible = false;
                return;
            }

            if ( Service.PartyList.Any(p => p.Statuses.All(s => s.StatusId != WellFedStatusID)) )
            {
                Visible = true;
            }
            else
            {
                Visible = Service.PartyList
                    .Select(p => p.Statuses.First(s => s.StatusId == WellFedStatusID))
                    .Any(s => s.RemainingTime < Service.Configuration.FoodEarlyWarningTime); ;
            }
        }

        protected override void UpdateSolo()
        {
            bool currentlyInCombat = Service.Condition[ConditionFlag.InCombat];

            // Don't show if in combat
            if (currentlyInCombat)
            {
                Visible = false;
                return;
            }

            var player = Service.ClientState.LocalPlayer;
            if (player == null) return;

            if ( player.StatusList.Any(s => s.StatusId == WellFedStatusID) )
            {
                Visible = player.StatusList
                    .First(s => s.StatusId == WellFedStatusID)
                    .RemainingTime < Service.Configuration.FoodEarlyWarningTime;
            }
            else
            {
                Visible = true;
            }
        }
    }
}
