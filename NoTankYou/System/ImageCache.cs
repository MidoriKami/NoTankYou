using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiScene;

namespace NoTankYou.Utilities;

public class ImageCache : IDisposable
{
    private static ImageCache? _instance;
    public static ImageCache Instance => _instance ??= new ImageCache();

    private readonly Dictionary<string, TextureWrap?> imageTextures = new();

    public static void Cleanup() => Instance.Dispose();
    
    public void Dispose()
    {
        foreach (var texture in imageTextures.Values)
        {
            texture?.Dispose();
        }
        
        imageTextures.Clear();
    }

    private void LoadImageTexture(string imagePath)
    {
        Task.Run(() =>
        {
            try
            {
                var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
                var combinedPath = Path.Combine(assemblyLocation, "images", imagePath);
                imageTextures[imagePath] = Service.PluginInterface.UiBuilder.LoadImage(combinedPath);
            }
            catch (Exception exception)
            {
                PluginLog.LogError(exception, $"Failed loading image: {imagePath}");
            }
        });
    }
    
    public TextureWrap? GetImage(string imagePath)
    {
        if (imageTextures.TryGetValue(imagePath, out var textureWrap)) return textureWrap;
        
        imageTextures.Add(imagePath, null);
        LoadImageTexture(imagePath);

        return imageTextures[imagePath];
    }
}