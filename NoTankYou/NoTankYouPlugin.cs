using Dalamud.Plugin;
using KamiLib;
using NoTankYou.Overlays;
using NoTankYou.System;
using NoTankYou.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin
{
    public string Name => "NoTankYou";
    private const string ShorthandCommand = "/nty";

    public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
    {
        // Create Static Services for use everywhere
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name, () => Service.ConfigurationManager.Save());
        Service.Localization = new LocalizationManager();
        
        // Systems that have no dependencies
        Service.FontManager = new FontManager();
        Service.PartyListAddon = new PartyListAddon();

        // Dependent systems below
        Service.ConfigurationManager = new ConfigurationManager();
        Service.ModuleManager = new ModuleManager();
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiCommon.WindowManager.AddWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new PartyListOverlayWindow());
        KamiCommon.WindowManager.AddWindow(new BannerOverlayWindow());
        KamiCommon.WindowManager.AddWindow(new PartyOverlayConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new BannerOverlayConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new BlacklistConfigurationWindow());
    }
        
    public void Dispose()
    {
        KamiCommon.Dispose();
        Service.Localization.Dispose();
        
        Service.FontManager.Dispose();
        Service.PartyListAddon.Dispose();

        Service.ConfigurationManager.Dispose();
    }
}