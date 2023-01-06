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
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("blu", ModuleName.BlueMage.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.BlueMage));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("cutscene", ModuleName.Cutscene.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Cutscene));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("dnc", ModuleName.Dancer.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Dancer));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("food", ModuleName.Food.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Food));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("fc", ModuleName.FreeCompany.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.FreeCompany));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("sge", ModuleName.Sage.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Sage));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("spiritbond", ModuleName.Spiritbond.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.SpiritBond));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("summoner", ModuleName.Summoner.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Summoner));
        KamiCommon.CommandManager.AddCommand(new GenericModuleCommand("tank", ModuleName.Tanks.GetTranslatedString(), Service.ConfigurationManager.CharacterConfiguration.Tank));

        
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