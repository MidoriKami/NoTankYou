using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiScene;

namespace NoTankYou.System;

public class IconManager : IDisposable 
{
    private readonly Dictionary<uint, TextureWrap?> IconTextures = new();

    public void Dispose() 
    {
        foreach (var texture in IconTextures.Values.Where(texture => texture != null)) 
        {
            texture?.Dispose();
        }

        IconTextures.Clear();
    }
        
    private void LoadIconTexture(uint iconId) 
    {
        Task.Run(() => {
            try {
                var iconTex = Service.DataManager.GetIcon(iconId);
                if (iconTex == null) return;
                
                var tex = Service.PluginInterface.UiBuilder.LoadImageRaw(iconTex.GetRgbaImageData(), iconTex.Header.Width, iconTex.Header.Height, 4);

                if (tex.ImGuiHandle != IntPtr.Zero) 
                {
                    IconTextures[iconId] = tex;
                } else {
                    tex.Dispose();
                }
            } 
            catch (Exception ex) 
            {
                PluginLog.LogError($"Failed loading texture for icon {iconId} - {ex.Message}");
            }
        });
    }
    
    public TextureWrap? GetIconTexture(uint iconId) 
    {
        if (IconTextures.ContainsKey(iconId)) return IconTextures[iconId];

        IconTextures.Add(iconId, null);
        LoadIconTexture(iconId);

        return IconTextures[iconId];
    }
}