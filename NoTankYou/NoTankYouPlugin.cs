using Dalamud.Plugin;
using KamiLib;
using KamiLib.System;
using NoTankYou.Localization;
using NoTankYou.System;
using NoTankYou.Views.Windows;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin
{
    public static NoTankYouSystem System = null!;

    public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, "NoTankYou");
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));

        System = new NoTankYouSystem();
        
        CommandController.RegisterMainCommand("/nty", "/notankyou");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }
        
    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}