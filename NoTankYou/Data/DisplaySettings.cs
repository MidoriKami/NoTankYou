using NoTankYou.Data.Overlays;

namespace NoTankYou.Data
{
    public class DisplaySettings
    {
        public PartyOverlaySettings PartyOverlay { get; set; } = new();
        public BannerOverlaySettings BannerOverlay { get; set; } = new();
    }
}
