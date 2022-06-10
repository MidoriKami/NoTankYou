using System.Numerics;
using ImGuiNET;

namespace NoTankYou.Utilities
{
    internal static class DrawUtilities
    {
        public static void VerticalLine()
        {
            var contentArea = ImGui.GetContentRegionAvail();
            var cursor = ImGui.GetCursorScreenPos();
            var drawList = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32(Colors.White);

            drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
        }

        public static void TextOutlined(Vector2 startingPosition, string text, float scale)
        {
            var outlineThickness = 1.0f * scale;

            DrawText(startingPosition + new Vector2(-outlineThickness, -outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(0.0f, -outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(outlineThickness, -outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(outlineThickness, 0.0f), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(outlineThickness, outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(0.0f, outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(-outlineThickness, outlineThickness), text, Colors.Black, scale);
            DrawText(startingPosition + new Vector2(-outlineThickness, 0.0f), text, Colors.Black, scale);

            DrawText(startingPosition, text, Colors.White, scale);
        }

        public static void DrawIconWithName(Vector2 drawPosition, uint iconID, string name, float scale, bool drawText = true)
        {
            if (!Service.FontManager.GameFont.Available) return;

            var icon = Service.IconManager.GetIconTexture(iconID);
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

                    TextOutlined(drawPosition + textOffset, name, scale / 2.75f);
                }
            }
        }

        public static Vector2 CalculateTextSize(string text, float scale)
        {
            if(!Service.FontManager.GameFont.Available) return Vector2.Zero;

            var fontSize = Service.FontManager.GameFont.ImFont.FontSize;
            var textSize = ImGui.CalcTextSize(text);
            var fontScalar = 60.0f / textSize.Y;

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
}