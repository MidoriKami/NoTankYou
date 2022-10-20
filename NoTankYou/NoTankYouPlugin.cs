using Dalamud.Plugin;
using NoTankYou.System;

namespace NoTankYou;

public sealed class NoTankYouPlugin : IDalamudPlugin
{
    public string Name => "NoTankYou";

    public NoTankYouPlugin(DalamudPluginInterface pluginInterface)
    {
        // Create Static Services for use everywhere
        pluginInterface.Create<Service>();

        // Systems that have no dependencies
        Service.FontManager = new FontManager();
        Service.LocalizationManager = new LocalizationManager();
        Service.PartyListAddon = new PartyListAddon();
        Service.DutyEventManager = new DutyEventManager();
        Service.ContextManager = new ContextManager();
        Service.IconManager = new IconManager();
        Service.DutyLists = new DutyLists();

        // Dependent systems below
        Service.ConfigurationManager = new ConfigurationManager();
        Service.ModuleManager = new ModuleManager();
        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();
    }
        
    public void Dispose()
    {
        Service.FontManager.Dispose();
        Service.LocalizationManager.Dispose();
        Service.PartyListAddon.Dispose();
        Service.DutyEventManager.Dispose();
        Service.ContextManager.Dispose();
        Service.IconManager.Dispose();
        Service.DutyLists.Dispose();

        Service.ConfigurationManager.Dispose();
        Service.WindowManager.Dispose();
        Service.CommandSystem.Dispose();
    }
}