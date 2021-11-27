using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NoTankYou
{
    public class TerritoryManager : IDisposable
    {
        private uint[] AllianceRaidTerritoryTypes;
        private WarningWindow warningWindow;

        public TerritoryManager(WarningWindow warningWindow)
        {
            this.warningWindow = warningWindow;

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            InitalizeAllianceRaidTerritoryTypeList();
        }
        private void InitalizeAllianceRaidTerritoryTypeList()
        {
            // Get database of territory types
            var territorySheets = Service.DataManager.GetExcelSheet<TerritoryType>();

            // check that we actually got a table,
            // because visual studio seems to think theres a high chance we might not have gotten one
            if (territorySheets != null)
            {
                // Select the rows that contain a IntendedUse of 8
                // IntendedUse value of 8 is Alliance Raid
                AllianceRaidTerritoryTypes = territorySheets.Where(r => r.TerritoryIntendedUse == 8).Select(r => r.RowId).ToArray();
            }
        }

        // Triggers on Map Change
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            // Debug data
            Service.Chat.Print($"[NoTankYou] Territory Changed. NewID: {e}");

            // Determine current state
            bool disabledBecauseAllianceRaid = IsAllianceRaid() && Service.Configuration.DisableInAllianceRaid;
            bool disabledBecauseBlacklist = Service.Configuration.TerritoryBlacklist.Contains(e);

            // Either enable or disable the banner depending on state
            if (disabledBecauseAllianceRaid || disabledBecauseBlacklist)
            {
                warningWindow.Active = false;
            }
            else
            {
                warningWindow.Active = true;
            }

            // Delay rendering for InstanceLoadDelayTime milliseconds
            // Territory Change and party formation happens BEFORE the loading screen appears
            // We must delay rendering or itll show awkwardly during loading screen, also gives the tank a "grace period"
            warningWindow.Delayed = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { warningWindow.Delayed = false; });
        }

        // Only checks the currently occupied territory for match in alliance raid database
        public bool IsAllianceRaid()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            return AllianceRaidTerritoryTypes.Contains(currentTerritory);
        }

        public void Dispose()
        {
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }
    }
}
