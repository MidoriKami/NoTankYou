using System.Numerics;
using NoTankYou.Enums;

namespace NoTankYou.Data.Overlays
{
    public class BannerOverlaySettings
    {
        public bool Enabled = false;
        public float Scale = 1.0f;
        public int WarningCount = 8;
        public BannerOverlayDisplayMode Mode = BannerOverlayDisplayMode.List;
        public Vector2 Position = new(512.0f, 512.0f);
        public bool Reposition = false;
        public bool WarningShield = true;
        public bool WarningText = true;
        public bool Icon = true;
    }
}
