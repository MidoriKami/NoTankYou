using CheapLoc;

namespace NoTankYou.Localization
{
    #region Modules
    public class Modules
    {
        public Tank Tank { get; set; } = new();
        public Dancer Dancer { get; set; } = new();
        public Food Food { get; set; } = new();
        public Sage Sage { get; set; } = new();
        public Scholar Scholar { get; set; } = new();
        public Summoner Summoner { get; set; } = new();
    }

    public class Tank
    {
        public readonly string Label = Loc.Localize("Tank_Label", "Tanks");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Tank_ConfigurationPanelLabel", "Tanks Configuration");
        public readonly string WarningText = Loc.Localize("Tank_WarningText", "Tank Stance Missing");
    }

    public class Dancer
    {
        public readonly string Label = Loc.Localize("Dancer_Label", "Dancer");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Dancer_ConfigurationPanelLabel", "Dancer Configuration");
        public readonly string WarningText = Loc.Localize("Dancer_WarningText", "Dance Partner Missing");
    }

    public class Food
    {
        public readonly string Label = Loc.Localize("Food_Label", "Food");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Food_ConfigurationPanelLabel", "Food Configuration");
        public readonly string WarningText = Loc.Localize("Food_WarningText", "Food Missing");
    }

    public class Sage
    {
        public readonly string Label = Loc.Localize("Sage_Label", "Sage");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Sage_ConfigurationPanelLabel", "Sage Configuration");
        public readonly string WarningText = Loc.Localize("Sage_WarningText", "Kardia Partner Missing");
    }

    public class Scholar
    {
        public readonly string Label = Loc.Localize("Scholar_Label", "Scholar");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Scholar_ConfigurationPanelLabel", "Scholar Configuration");
        public readonly string WarningText = Loc.Localize("Scholar_WarningText", "Faerie Summon Missing");
    }

    public class Summoner
    {
        public readonly string Label = Loc.Localize("Summoner_Label", "Summoner");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Summoner_ConfigurationPanelLabel", "Summoner Configuration");
        public readonly string WarningText = Loc.Localize("Summoner_WarningText", "Pet Summon Missing");
    }

    #endregion

    #region Common
    public class Common
    {
        public Tabs Tabs { get; set; } = new();
        public TabItems TabItems { get; set; } = new();
        public Labels Labels { get; set; } = new();
    }

    public class Labels
    {
        public readonly string About = Loc.Localize("Labels_About", "About");
        public readonly string Options = Loc.Localize("Labels_Options", "Options");
        public readonly string Description = Loc.Localize("Labels_Description", "Description");
        public readonly string TechnicalDescription = Loc.Localize("Labels_TechnicalDescription", "Technical Description");
    }

    public class Tabs
    {
        public readonly string Settings = Loc.Localize("Tabs_Settings", "Settings");
        public readonly string SettingsDescription = Loc.Localize("Tabs_SettingsDescription", "Basic settings options");

        public readonly string Display = Loc.Localize("Tabs_Display", "Display");
        public readonly string DisplayDescription = Loc.Localize("Tabs_DisplayDescription", "Configure how warnings are displayed");

        public readonly string Modules = Loc.Localize("Tabs_Modules", "Modules");
        public readonly string ModulesDescription = Loc.Localize("Tabs_ModulesDescription", "Configure each module");
    }

    public class TabItems
    {
        public PartyOverlay PartyOverlay { get; set; } = new();
    }

    public class PartyOverlay
    {
        public readonly string Label = Loc.Localize("PartyOverlay_Label", "Party Frame Overlay");
        public readonly string Description = Loc.Localize("PartyOverlay_Description", "The party frame overlay I'll just write this later, if I forgot let me know.");
        public readonly string TechnicalDescription = Loc.Localize("PartyOverlay_TechnicalDescription", "Technical words, if I forgot this too, also let me know.");
    }
    #endregion

    #region Configuration
    public class Configuration
    {
        public readonly string NoSelection = Loc.Localize("Configuration_NoSelection", "Select an item to configure in the left pane");
        public readonly string Enable = Loc.Localize("Configuration_Enable", "Enable");
    }
    #endregion

    #region Commands
    public class Commands
    {
        public readonly string On = Loc.Localize("On", "on");
        public readonly string Show = Loc.Localize("Show", "show");
        public readonly string Off = Loc.Localize("Off", "off");
        public readonly string Hide = Loc.Localize("Hide", "hide");
        public readonly string Close = Loc.Localize("Close", "close");
        public readonly string Open = Loc.Localize("Open", "open");
        public readonly string Toggle = Loc.Localize("Toggle", "toggle");
        public readonly string Help = Loc.Localize("Help", "help");
        public readonly string Core = Loc.Localize("Core", "Core");
    }
    #endregion

    internal static class Strings
    {
        public static Modules Modules { get; set; } = new();
        public static Common Common { get; set; } = new();
        public static Configuration Configuration { get; set; } = new();
        public static Commands Commands { get; set; } = new();

        public static void ReInitialize()
        {
            Modules = new();
            Common = new();
            Configuration = new();
            Commands = new();
        }
    }
}
