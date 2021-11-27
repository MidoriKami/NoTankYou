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
            var territorySheets = Service.DataManager.GetExcelSheet<TerritoryType>();

            if (territorySheets != null)
            {
                // IntendedUse value of 8 is Alliance Raid
                AllianceRaidTerritoryTypes = territorySheets.Where(r => r.TerritoryIntendedUse == 8).Select(r => r.RowId).ToArray();
            }
        }
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            Service.Chat.Print($"Territory Changed. NewID:{e}");

            bool disabledBecauseAllianceRaid = IsAllianceRaid() && Service.Configuration.DisableInAllianceRaid;
            bool disabledBecauseBlacklist = Service.Configuration.TerritoryBlacklist.Contains(e);

            if (disabledBecauseAllianceRaid || disabledBecauseBlacklist)
            {
                warningWindow.Active = false;
            }

            warningWindow.Delayed = true;
            Task.Delay(Service.Configuration.InstanceLoadDelayTime).ContinueWith(t => { warningWindow.Delayed = false; });
        }
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
