using Dalamud.Plugin;
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

        KamiLib.KamiLib.Initialize(pluginInterface, Name, () => Service.ConfigurationManager.Save());
        
        // Systems that have no dependencies
        Service.FontManager = new FontManager();
        Service.LocalizationManager = new LocalizationManager();
        Service.PartyListAddon = new PartyListAddon();
        Service.DutyEventManager = new DutyEventManager();
        Service.ContextManager = new ContextManager();
        Service.DutyLists = new DutyLists();
        
        // Dependent systems below
        Service.ConfigurationManager = new ConfigurationManager();
        Service.ModuleManager = new ModuleManager();
        
        KamiLib.KamiLib.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiLib.KamiLib.WindowManager.AddWindow(new ConfigurationWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new PartyListOverlayWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new BannerOverlayWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new PartyOverlayConfigurationWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new BannerOverlayConfigurationWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new BlacklistConfigurationWindow());
        KamiLib.KamiLib.WindowManager.AddWindow(new DebugWindow());
    }
        
    public void Dispose()
    {
        KamiLib.KamiLib.Dispose();
        
        Service.FontManager.Dispose();
        Service.LocalizationManager.Dispose();
        Service.PartyListAddon.Dispose();
        Service.DutyEventManager.Dispose();
        Service.ContextManager.Dispose();
        Service.DutyLists.Dispose();

        Service.ConfigurationManager.Dispose();
    }
}