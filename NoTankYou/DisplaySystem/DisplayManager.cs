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

        private readonly HashSet<uint> PvPTerritoryBlacklist;
        private readonly HashSet<uint> AllianceRaidTerritories;
        private readonly HashSet<uint> GATETerritories;

        public static List<Configuration.ModuleSettings> AllModuleSettings = new()
        {
            Service.Configuration.FaerieSettings,
            Service.Configuration.KardionSettings,
            Service.Configuration.DancePartnerSettings,
            Service.Configuration.TankStanceSettings,
            Service.Configuration.SummonerSettings
        };

        public DisplayManager()
        {
            Banners.Add(new FaerieBanner());
            Banners.Add(new KardionBanner());
            Banners.Add(new DancePartnerBanner());
            Banners.Add(new TankStanceBanner());
            Banners.Add(new SummonerBanner());
            Banners.Add(new BlueMageTankStance());

            foreach (var banner in Banners)
            {
                Service.WindowSystem.AddWindow(banner);
            }

            // Battalion MainMode 4 = PvP
            PvPTerritoryBlacklist = Service.DataManager.GetExcelSheet<TerritoryType>()
                            !.Where(r => r.BattalionMode is 4 or 6)
                            .Select(r => r.RowId)
                            .ToHashSet();

            // Territory Intended Use 8 = Alliance Raid
            AllianceRaidTerritories = Service.DataManager.GetExcelSheet<TerritoryType>()
                            !.Where(r => r.TerritoryIntendedUse is 8)
                            .Select(r => r.RowId)
                            .ToHashSet();

            // ContentFinderCondition 19 = Golden Saucer
            GATETerritories = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(c => c.ContentType.Row is 19)
                .Select(r => r.TerritoryType.Row)
                .ToHashSet();

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }
        private void OnTerritoryChanged(object? sender, ushort e)
        {
            try
            {
                bool movingToGoldenSaucerEvent = GATETerritories.Contains(e);
                bool movingToBlacklistedTerritory = Service.Configuration.TerritoryBlacklist.Contains(e);
                bool movingToAllianceRaid = AllianceRaidTerritories.Contains(e);
                bool movingToPvPTerritory = PvPTerritoryBlacklist.Contains(e);

                bool shouldDisable = movingToPvPTerritory ||
                                     (movingToAllianceRaid && Service.Configuration.DisableInAllianceRaid) ||
                                     movingToBlacklistedTerritory ||
                                     movingToGoldenSaucerEvent;

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
                banner.UpdateImageSize();
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
