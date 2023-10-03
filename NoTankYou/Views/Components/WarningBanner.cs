using System.Drawing;
using System.Numerics;
using Dalamud.Interface.Internal;
using ImGuiNET;
using NoTankYou.DataModels;
using NoTankYou.Models;
using NoTankYou.Utilities;

namespace NoTankYou.Views.Components;

public class WarningBanner
{
    private static IDalamudTextureWrap? WarningIcon => ImageCache.Instance.GetImage("BigWarning.png");
    public static void Draw(Vector2 drawPosition, WarningState? warning, BannerConfig settings)
    {
        if (warning is null) return;
        
        drawPosition += ImGui.GetMainViewport().Pos;
        
        DrawWarningShield(ref drawPosition, settings);
        DrawWarningText(ref drawPosition, warning, settings);
        DrawActionIcon(ref drawPosition, warning, settings);
    }
    
    private static void DrawWarningShield(ref Vector2 startingPosition, BannerConfig displaySettings)
    {
        var drawList = ImGui.GetBackgroundDrawList();
        
        if (displaySettings.WarningShield && WarningIcon is not null)
        {
            var imageSize = new Vector2(WarningIcon.Width, WarningIcon.Height) * displaySettings.Scale;
    
            drawList.AddImage(WarningIcon.ImGuiHandle, startingPosition, startingPosition + imageSize);
            
            startingPosition = startingPosition with
            {
                X = startingPosition.X + imageSize.X + 10.0f * displaySettings.Scale, Y = startingPosition.Y + 4.0f,
            };
        }
    }

    private static void DrawWarningText(ref Vector2 startingPosition, WarningState warningState, BannerConfig displaySettings)
    {
        if (displaySettings.WarningText)
        {
            DrawUtilities.TextOutlined(startingPosition, warningState.Message, displaySettings.Scale, KnownColor.White);

            var textSize = DrawUtilities.CalculateTextSize(warningState.Message, displaySettings.Scale);
            startingPosition = startingPosition with {X = startingPosition.X + textSize.X};

            if (displaySettings.PlayerNames)
            {
                textSize = DrawUtilities.CalculateTextSize(warningState.SourcePlayerName, displaySettings.Scale);
                startingPosition = startingPosition with
                {
                    X = startingPosition.X - textSize.X / 2.0f, Y = startingPosition.Y + textSize.Y,
                };
                DrawUtilities.TextOutlined(startingPosition, warningState.SourcePlayerName, displaySettings.Scale / 2.0f, KnownColor.White);
                startingPosition = startingPosition with
                {
                    X = startingPosition.X + textSize.X / 2.0f, Y = startingPosition.Y - textSize.Y,
                };
            }
        }
    }

    private static void DrawActionIcon(ref Vector2 startingPosition, WarningState warningState, BannerConfig displaySettings)
    {
        if (displaySettings.Icon)
        {
            DrawUtilities.DrawIconWithName(startingPosition, warningState.IconId, warningState.IconLabel, displaySettings.Scale, displaySettings.ShowActionName);
        }
    }
}