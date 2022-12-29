using System;
using Dalamud.Interface.GameFonts;

namespace NoTankYou.System;

public class FontManager : IDisposable
{
    public GameFontHandle GameFont { get; }

    public FontManager()
    {
        GameFont = Service.PluginInterface.UiBuilder.GetGameFontHandle( new GameFontStyle( GameFontFamily.Axis, 52.0f ) );
    }

    public void Dispose()
    {
        GameFont.Dispose();
    }
}