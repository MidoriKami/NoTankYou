using System;
using Dalamud.Interface.GameFonts;

namespace NoTankYou.System
{
    public class FontManager : IDisposable
    {
        public GameFontHandle GameFont { get; private set; }

        public FontManager()
        {
            GameFont = Service.PluginInterface.UiBuilder.GetGameFontHandle( new GameFontStyle( GameFontFamilyAndSize.Axis18 ) );
        }

        public void Dispose()
        {
            GameFont.Dispose();
        }
    }
}
