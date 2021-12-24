using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DisplaySystem.Banners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoTankYou.DisplaySystem
{
    internal class DisplayManager : IDisposable
    {
        private readonly List<WarningBanner> Banners = new();

        private readonly List<uint> PvPTerritoryBlacklist;
        private readonly List<uint> AllianceRaidTerritories;

        public DisplayManager()
        {
            Banners.Add(new FaerieBanner());
            Banners.Add(new KardionBanner());
            Banners.Add(new DancePartnerBanner());
            Banners.Add(new TankStanceBanner());
            Banners.Add(new SummonerBanner());

            foreach (var banner in Banners)
            {
                Service.WindowSystem.AddWindow(banner);
            }

            // Battalion MainMode 4 = PvP
            PvPTerritoryBlacklist = Service.DataManager.GetExcelSheet<TerritoryType>()
                            !.Where(r => r.BattalionMode is 4)
                            .Select(r => r.RowId)
                            .ToList();

            // Territory Intended Use 8 = Alliance Raid
            AllianceRaidTerritories = Service.DataManager.GetExcelSheet<TerritoryType>()
                            !.Where(r => r.TerritoryIntendedUse is 8)
                            .Select(r => r.RowId)
                            .ToList();

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            try
            {
                bool movingToBlacklistedTerritory = Service.Configuration.TerritoryBlacklist.Contains(e);
                bool movingToAllianceRaid = AllianceRaidTerritories.Contains(e);
                bool movingToPvPTerritory = PvPTerritoryBlacklist.Contains(e);
                bool shouldDisable = movingToPvPTerritory ||
                                     (movingToAllianceRaid && Service.Configuration.DisableInAllianceRaid) ||
                                     movingToBlacklistedTerritory;

                if (shouldDisable)
                {
                    foreach (var banner in Banners)
                    {
                        banner.Disabled = true;
                    }
                }
                else
                {
                    foreach (var banner in Banners)
                    {
                        banner.Disabled = false;
                    }
                }


                // Skip delaying if we are the reason we are re-evaluating
                if (sender == this) return;

                // Delay Displaying Warnings Until Grace Period Passes
                foreach (var banner in Banners)
                {
                    banner.Paused = true;
                }

                Task.Delay(Service.Configuration.TerritoryChangeDelayTime).ContinueWith(t =>
                {
                    foreach (var banner in Banners)
                    {
                        banner.Paused = false;
                    }
                });
            }
            catch (NullReferenceException ex)
            {
                PluginLog.Error(ex.Message);
            }
        }

        internal void Update()
        {
            // Slow update rate to conserve performance
            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % (ulong)Service.Configuration.NumberOfWaitFrames != 0) return;

            if (Service.Configuration.ForceWindowUpdate)
            {
                ForceWindowUpdate();
                Service.Configuration.ForceWindowUpdate = false;
            }

            foreach (var banner in Banners)
            {
                banner.Update();
            }
        }

        private void ForceWindowUpdate()
        {
            foreach (var banner in Banners)
            {
                banner.ChangeImageSize(Service.Configuration.ImageSize);
            }

            OnTerritoryChanged(this, Service.ClientState.TerritoryType);
        }

        public void Dispose()
        {
            foreach (var banner in Banners)
            {
                banner.Dispose();
            }
        }
    }
}
