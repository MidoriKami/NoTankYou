using System.Numerics;
using NoTankYou.Utilities;

namespace NoTankYou.Data.Overlays
{
    public class PartyOverlaySettings
    {
        public bool Enabled = false;
        public bool WarningText = true;
        public bool PlayerName = true;
        public bool JobIcon = true;
        public bool FlashingEffects = true;
        public Vector4 WarningTextColor = Colors.SoftRed;
        public Vector4 WarningOutlineColor = Colors.Red;
    }
}
