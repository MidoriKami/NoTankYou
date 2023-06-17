using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using NoTankYou.System;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.Utilities;

public class DrawUtilities
{
    public static void TextOutlined(Vector2 startingPosition, string text, float scale, KnownColor color)
    {
        startingPosition = new Vector2(MathF.Ceiling(startingPosition.X), MathF.Ceiling(startingPosition.Y));

        var outlineThickness = (int)MathF.Ceiling(1.0f * scale);
        
        for (var x = -outlineThickness; x <= outlineThickness; ++x)
        {
            for (var y = -outlineThickness; y <= outlineThickness; ++y)
            {
                if (x == 0 && y == 0) continue;

                DrawText(startingPosition + new Vector2(x, y), text, KnownColor.Black, scale);
            }
        }

        DrawText(startingPosition, text, color, scale);
    }
    
    public static Vector2 CalculateTextSize(string text, float scale)
    {
        ImGui.PushFont(NoTankYouSystem.Axis56.ImFont);
        var textSize = ImGui.CalcTextSize(text) / ImGuiHelpers.GlobalScale;
        ImGui.PopFont();

        return new Vector2(textSize.X, textSize.Y) * scale;
    }
    
    private static void DrawText(Vector2 drawPosition, string text, KnownColor color, float scale)
    {
        var font = NoTankYouSystem.Axis56.ImFont;
        var drawList = ImGui.GetBackgroundDrawList();

        // Debug Outline for Text
            // var stringSize = CalculateTextSize(text, scale);
            // drawList.AddRect(drawPosition, drawPosition + stringSize, ImGui.GetColorU32(KnownColor.Green.AsVector4()));
        
        drawList.AddText(font, font.FontSize * scale, drawPosition, ImGui.GetColorU32(color.AsVector4()), text);
    }
    
    public static void DrawIconWithName(Vector2 drawPosition, uint iconId, string iconLabel, float scale, bool showActionName)
    {
        if (!NoTankYouSystem.Axis56.Available) return;
        
        if (IconCache.Instance.GetIcon(iconId) is { } icon)
        {
            var drawList = ImGui.GetBackgroundDrawList();

            var imagePadding = new Vector2(20.0f, 10.0f) * scale;
            var imageSize = new Vector2(50.0f, 50.0f) * scale;

            drawPosition += imagePadding;

            drawList.AddImage(icon.ImGuiHandle, drawPosition, drawPosition + imageSize);

            if (showActionName)
            {
                drawPosition.X += imageSize.X / 2.0f;
                drawPosition.Y += imageSize.Y + 2.0f * scale;

                var textSize = CalculateTextSize(iconLabel, scale / 2.75f);
                var textOffset = new Vector2(0.0f, 17.5f) * scale;

                drawPosition.X -= textSize.X / 2.0f;

                TextOutlined(drawPosition + textOffset, iconLabel, scale / 2.75f, KnownColor.White);
            }
        }
    }
}