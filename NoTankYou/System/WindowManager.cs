using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using NoTankYou.UserInterface.Windows;

namespace NoTankYou.System;

public class WindowManager : IDisposable
{
    private readonly WindowSystem WindowSystem = new("NoTankYou");

    private readonly List<Window> Windows = new()
    {
        new ConfigurationWindow(),
        new PartyListOverlayWindow(),
        new BannerOverlayWindow(),
        new PartyOverlayConfigurationWindow(),
        new BannerOverlayConfigurationWindow(),
        new BlacklistConfigurationWindow(),
    };

    public WindowManager()
    {
        foreach (var window in Windows)
        {
            WindowSystem.AddWindow(window);
        }

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    private void DrawUI() => WindowSystem.Draw();

    private void DrawConfigUI() => Windows[0].IsOpen = true;

    public T? GetWindowOfType<T>()
    {
        return Windows.OfType<T>().FirstOrDefault();
    }

    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        foreach (var window in Windows.OfType<IDisposable>())
        {
            window.Dispose();
        }

        WindowSystem.RemoveAllWindows();
    }
}