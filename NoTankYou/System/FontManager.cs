using System;
using System.IO;
using System.Threading.Tasks;
using ImGuiNET;

namespace NoTankYou.System
{
    public class FontManager : IDisposable
    {
        public ImFontPtr Font { get; private set; }
        public bool FontBuilt { get; private set; }

        public FontManager()
        {
            Service.PluginInterface.UiBuilder.BuildFonts += BuildFont;
            Task.Delay(1000).ContinueWith(_ => Service.PluginInterface.UiBuilder.RebuildFonts());
        }

        public void Dispose()
        {
            Service.PluginInterface.UiBuilder.BuildFonts -= BuildFont;
            Service.PluginInterface.UiBuilder.RebuildFonts();
        }

        private void BuildFont() 
        {
            var fontFile = Path.Combine(Service.PluginInterface.DalamudAssetDirectory.FullName, "UIRes", "NotoSansCJKjp-Medium.otf");

            Font = ImGui.GetIO().Fonts.AddFontFromFileTTF(fontFile, 68);
            FontBuilt = true;
        }
    }
}
