using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace NoTankYou.Models;

[Category("DisplayColors", 3)]
public interface IPartyListDisplayColors
{
    [ColorConfig("TextColor", 1.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TextColor { get; set; }

    [ColorConfig("OutlineColor", 1.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 OutlineColor { get; set; }
}