using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models.BannerConfiguration;

[Category("Positioning", 1)]
public interface IBannerPositioning
{
    [BoolConfig("WindowDragging")]
    public bool CanDrag { get; set; }
    
    [Vector2Config("Position")]
    public Vector2 WindowPosition { get; set; }

    [FloatConfig("Scale", 0.25f, 3.0f)]
    public float Scale { get; set; }
}