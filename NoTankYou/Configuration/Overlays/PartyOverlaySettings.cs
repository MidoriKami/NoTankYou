using System.Numerics;
using NoTankYou.Configuration.Components;
using NoTankYou.Utilities;

namespace NoTankYou.Configuration.Overlays;

public class PartyOverlaySettings
{
    public Setting<bool> WarningText = new(true);
    public Setting<bool> PlayerName = new(true);
    public Setting<bool> JobIcon = new(true);
    public Setting<bool> FlashingEffects = new(true);
    public Setting<Vector4> WarningTextColor = new (Colors.SoftRed);
    public Setting<Vector4> WarningOutlineColor = new (Colors.Red);
    public Setting<bool> DisableInSanctuary = new(false);
}