using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoTankYou
{
    public class TerritoryManager : IDisposable
    {
        private List<uint> AllianceRaidTerritoryTypes = new();
        private List<uint> PvPZones = new();
        private WarningWindow warningWindow;

        public TerritoryManager(WarningWindow warningWindow)
        {
            this.warningWindow = warningWindow;

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            InitalizeAllianceRaidTerritoryTypeList();
            InitializePvPTerritoryTypeList();
        }

        private void InitializePvPTerritoryTypeList()
        {
            var territoryTypes = Service.DataManager.GetExcelSheet<TerritoryType>();

            // BattalionMode 4 is PvP
            PvPZones = territoryTypes
                !.Where(r => r.BattalionMode is 4)
                .Select(r => r.RowId)
                .ToList();
        }

        private void InitalizeAllianceRaidTerritoryTypeList()
        {
            // Get database of territory types
            var territorySheets = Service.DataManager.GetExcelSheet<TerritoryType>();

            // Select the rows that contain a IntendedUse of 8
            // IntendedUse value of 8 is Alliance Raid
            AllianceRaidTerritoryTypes = territorySheets
                !.Where(r => r.TerritoryIntendedUse is 8)
                .Select(r => r.RowId)
                .ToList();
        }

        // Triggers on Map Change
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            UpdateWindowStatus();

            // Delay rendering for InstanceLoadDelayTime milliseconds
            // Territory Change and party formation happens BEFORE the loading screen appears
            // We must delay rendering or itll show awkwardly during loading screen, also gives the tank a "grace period"
            warningWindow.Delayed = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { warningWindow.Delayed = false; });
        }

        public void UpdateWindowStatus()
        {
            var currentTerritory = Service.ClientState.TerritoryType;

            // Determine current state
            bool newTerritoryIsAllianceRaid = IsAllianceRaid(currentTerritory);
            bool disabledBecauseAllianceRaid = newTerritoryIsAllianceRaid && Service.Configuration.DisableInAllianceRaid;
            bool disabledBecauseBlacklist = Service.Configuration.TerritoryBlacklist.Contains(currentTerritory);
            bool disableBecausePvPZone = IsPvPZone(currentTerritory);

            // Either enable or disable the banner depending on state
            if (disabledBecauseAllianceRaid || disabledBecauseBlacklist || disableBecausePvPZone)
            {
                warningWindow.Active = false;
            }
            else
            {
                warningWindow.Active = true;
            }
        }

        private bool IsPvPZone(ushort currentTerritory)
        {
            return PvPZones.Contains(currentTerritory);
        }

        private bool IsPvPZone(ushort currentTerritory)
        {
            return PvPZones.Contains(currentTerritory);
        }

        // Only checks the currently occupied territory for match in alliance raid database
        public bool IsAllianceRaid(ushort territory)
        {
            return AllianceRaidTerritoryTypes.Contains(territory);
        }

        public void Dispose()
        {
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }
    }
}
