using Dalamud.Plugin;
using KamiLib;
using NoTankYou.Commands;
using NoTankYou.DataModels;
using NoTankYou.System;
using NoTankYou.UserInterface.OverlayWindows;
using NoTankYou.UserInterface.Windows;

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
        LocalizationManager.Instance.Initialize();
        
        // Systems that have no dependencies
        Service.FontManager = new FontManager();
        Service.PartyListAddon = new PartyListAddon();

        // Dependent systems below
        Service.ConfigurationManager = new ConfigurationManager();
        Service.ModuleManager = new ModuleManager();
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new PartyListOverlayWindow());
        KamiCommon.WindowManager.AddWindow(new BannerOverlayWindow());
        KamiCommon.WindowManager.AddWindow(new PartyOverlayConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new BannerOverlayConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new BlacklistConfigurationWindow());
    }
        
    public void Dispose()
    {
        KamiCommon.Dispose();
        
        Service.FontManager.Dispose();
        Service.PartyListAddon.Dispose();
        Service.ConfigurationManager.Dispose();
        
        LocalizationManager.Cleanup();
    }
}