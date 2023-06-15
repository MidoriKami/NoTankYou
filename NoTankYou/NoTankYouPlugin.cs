using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using KamiLib;
using KamiLib.Commands;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.Views.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin
{
    public string Name => "NoTankYou";
    
    public static NoTankYouSystem System = null!;

    public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
    {
        // Create Static Services for use everywhere
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));

        System = new NoTankYouSystem();
        
        CommandController.RegisterMainCommand("/nty", "/notankyou");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
        // KamiCommon.WindowManager.AddWindow(new PartyListOverlayWindow());
        // KamiCommon.WindowManager.AddWindow(new BannerOverlayWindow());

        unsafe
        {
            PluginLog.Debug($"GroupManager: {new nint(GroupManager.Instance()):X8}");
        }
    }
        
    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}