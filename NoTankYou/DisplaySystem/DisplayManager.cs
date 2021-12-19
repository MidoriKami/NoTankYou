using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoTankYou.DisplaySystem.Banners;

namespace NoTankYou.DisplaySystem
{
    internal class DisplayManager : IDisposable
    {
        private readonly FaerieBanner FaerieBanner;
        private readonly KardionBanner KardionBanner;
        private readonly DancePartnerBanner DancePartnerBanner;
        private readonly TankStanceBanner TankStanceBanner;

        private readonly List<uint> PvPTerritoryBlacklist;
        private readonly List<uint> AllianceRaidTerritories;

        public DisplayManager(
            ImGuiScene.TextureWrap dancePartnerImage,
            ImGuiScene.TextureWrap faerieImage,
            ImGuiScene.TextureWrap kardionImage,
            ImGuiScene.TextureWrap tankStanceImage)
        {
            FaerieBanner = new FaerieBanner(faerieImage);
            KardionBanner = new KardionBanner(kardionImage);
            DancePartnerBanner = new DancePartnerBanner(dancePartnerImage);
            TankStanceBanner = new TankStanceBanner(tankStanceImage);

            Service.WindowSystem.AddWindow(DancePartnerBanner);
            Service.WindowSystem.AddWindow(KardionBanner);
            Service.WindowSystem.AddWindow(FaerieBanner);
            Service.WindowSystem.AddWindow(TankStanceBanner);

            // Battalion Mode 4 = PvP
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
            bool movingToBlacklistedTerritory = Service.Configuration.TerritoryBlacklist.Contains(e);
            bool movingToAllianceRaid = AllianceRaidTerritories.Contains(e);
            bool movingToPvPTerritory = PvPTerritoryBlacklist.Contains(e);
            bool shouldDisable = movingToPvPTerritory || (movingToAllianceRaid && Service.Configuration.DisableInAllianceRaid) || movingToBlacklistedTerritory;

            if (shouldDisable)
            {
                FaerieBanner.Disabled = true;
                KardionBanner.Disabled = true;
                DancePartnerBanner.Disabled = true;
                TankStanceBanner.Disabled = true;
            }
            else
            {
                FaerieBanner.Disabled = false;
                KardionBanner.Disabled = false;
                DancePartnerBanner.Disabled = false;
                TankStanceBanner.Disabled = false;
            }

            // Delay Displaying Warnings Until Grace Period Passes
            FaerieBanner.Paused = true;
            KardionBanner.Paused = true;
            DancePartnerBanner.Paused = true;
            TankStanceBanner.Paused = true;

            Task.Delay(Service.Configuration.TerritoryChangeDelayTime).ContinueWith(t =>
            {
                FaerieBanner.Paused = false;
                KardionBanner.Paused = false;
                DancePartnerBanner.Paused = false;
                TankStanceBanner.Paused = false;
            });
        }

        internal void Update()
        {
            // Slow update rate to conserve performance
            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % (ulong) Service.Configuration.NumberOfWaitFrames != 0) return;

            if (Service.Configuration.ForceWindowUpdate)
            {
                var currentTerritory = Service.ClientState.TerritoryType;
                OnTerritoryChanged(this, currentTerritory);
                Service.Configuration.ForceWindowUpdate = false;
            }

            DancePartnerBanner.IsOpen = Service.Configuration.EnableDancePartnerBanner;
            KardionBanner.IsOpen = Service.Configuration.EnableKardionBanner;
            FaerieBanner.IsOpen = Service.Configuration.EnableFaerieBanner;
            TankStanceBanner.IsOpen = Service.Configuration.EnableTankStanceBanner;

            FaerieBanner.Update();
            KardionBanner.Update();
            DancePartnerBanner.Update();
            TankStanceBanner.Update();
        }

        public void Dispose()
        {
            FaerieBanner.Dispose();
            DancePartnerBanner.Dispose();
            KardionBanner.Dispose();
            TankStanceBanner.Dispose();
        }
    }
}
