using System;
using System.Numerics;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using NoTankYou.Windows;

namespace NoTankYou.Utilities;

internal static class DrawUtilities
{
    private static BannerOverlaySettings BannerOverlaySettings => Service.ConfigurationManager.CharacterConfiguration.BannerOverlay;
    
    public static void TextOutlined(Vector2 startingPosition, string text, float scale, Vector4 color)
    {
        startingPosition = startingPosition.Ceil();

        var outlineThickness = (int)MathF.Ceiling(BannerOverlaySettings.BorderThickness.Value * scale);
        
        for (var x = -outlineThickness; x <= outlineThickness; ++x)
        {
            for (var y = -outlineThickness; y <= outlineThickness; ++y)
            {
                if (x == 0 && y == 0) continue;

                DrawText(startingPosition + new Vector2(x, y), text, Colors.Black, scale);
            }
        }

        DrawText(startingPosition, text, color, scale);
    }

    public static void DrawIconWithName(Vector2 drawPosition, uint iconID, string name, float scale, bool drawText = true)
    {
        if (!Service.FontManager.GameFont.Available) return;

        var icon = IconCache.Instance.GetIcon(iconID);
        if (icon != null)
        {
            var drawList = ImGui.GetBackgroundDrawList();

            var imagePadding = new Vector2(20.0f, 10.0f) * scale;
            var imageSize = new Vector2(50.0f, 50.0f) * scale;

            drawPosition += imagePadding;

            drawList.AddImage(icon.ImGuiHandle, drawPosition, drawPosition + imageSize);

            if (drawText)
            {
                drawPosition.X += imageSize.X / 2.0f;
                drawPosition.Y += imageSize.Y + 2.0f * scale;

                var textSize = CalculateTextSize(name, scale / 2.75f);
                var textOffset = new Vector2(0.0f, 5.0f) * scale;

                drawPosition.X -= textSize.X / 2.0f;

                TextOutlined(drawPosition + textOffset, name, scale / 2.75f, Colors.White);
            }
        }
    }

    public static Vector2 CalculateTextSize(string text, float scale)
    {
        if(!Service.FontManager.GameFont.Available) return Vector2.Zero;

        var fontSize = Service.FontManager.GameFont.ImFont.FontSize;
        var textSize = ImGui.CalcTextSize(text);
        var fontScalar = 62.0f / textSize.Y;

        var textHeight = fontSize;
        var textWidth = textSize.X * fontScalar;

        return new Vector2(textWidth, textHeight) * scale;
    }

    private static void DrawText(Vector2 drawPosition, string text, Vector4 color, float scale, bool debug = false)
    {
        if (!Service.FontManager.GameFont.Available) return;
        var font = Service.FontManager.GameFont.ImFont;

        var drawList = ImGui.GetBackgroundDrawList();
        var stringSize = CalculateTextSize(text, scale);

        if(debug)
            drawList.AddRect(drawPosition, drawPosition + stringSize, ImGui.GetColorU32(Colors.Green));

        drawList.AddText(font, font.FontSize * scale, drawPosition, ImGui.GetColorU32(color), text);
    }
}

public static class VectorExtensions
{
    public static Vector2 Ceil(this Vector2 data)
    {
        return new Vector2(MathF.Ceiling(data.X), MathF.Ceiling(data.Y));
    }
}