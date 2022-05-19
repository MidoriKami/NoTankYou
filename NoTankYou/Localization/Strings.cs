using System.ComponentModel;
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
        public BlueMage BlueMage { get; set; } = new();
    }

    public class Tank
    {
        public readonly string Label = Loc.Localize("Tank_Label", "Tanks");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Tank_ConfigurationPanelLabel", "Tanks Configuration");
        public readonly string WarningText = Loc.Localize("Tank_WarningText", "Tank Stance Missing");
        public readonly string Description = Loc.Localize("Tank_Description", "Tanks use role actions commonly referred to as 'Tank Stances' to manage enemy aggro. If there are no tanks in the party with their 'Tank Stances' on then enemies may start attacking the healers or dps players directly, possibly leading to a party wipe.");
        public readonly string TechnicalDescription = Loc.Localize("Tank_TechnicalInformation", "This module will alert you if there are no tanks in the party with their tank stance on. If a tank with a stance on dies, this module will not consider their stance to be on until they are revived.");
        public readonly string DisableInAllianceRaid = Loc.Localize("Tank_DisableInAllianceRaid", "Disable in Alliance Raid");
    }

    public class Dancer
    {
        public readonly string Label = Loc.Localize("Dancer_Label", "Dancer");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Dancer_ConfigurationPanelLabel", "Dancer Configuration");
        public readonly string WarningText = Loc.Localize("Dancer_WarningText", "Dance Partner Missing");
        public readonly string Description = Loc.Localize("Dance_Description", "Dancers use a job action commonly referred to as 'Dance Partner' on another party member to give both the dancer and their partner a buff. This buff significantly increases both the dancer and their partners outgoing DPS.");
        public readonly string TechnicalDescription = Loc.Localize("Dance_TechnicalInformation", "This module will alert you when there are dancers that do not have the 'Closed Position' status effect on themselves. This effect is granted to the dancer when they also put 'Dance Partner' on another player.");
    }

    public class Food
    {
        public readonly string Label = Loc.Localize("Food_Label", "Food");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Food_ConfigurationPanelLabel", "Food Configuration");
        public readonly string WarningText = Loc.Localize("Food_WarningText", "Food Missing");
        public readonly string Description = Loc.Localize("Food_Description", "Consuming food grants the player the status effect 'Well Fed' while under this effect several of your stats will be increased for the duration of the buff.");
        public readonly string TechnicalDescription = Loc.Localize("Food_TechnicalInformation", "This module will alert you when the remaining duration of the 'Well Fed' status effect is less than the pre-configured value.");
        public readonly string EarlyWarningLabel = Loc.Localize("Food_EarlyWarningLabel", "Early Warning Time");
        public readonly string ZoneFilters = Loc.Localize("Food_ZoneFilters", "Zone Filters");
        public readonly string ZoneFiltersDescription = Loc.Localize("Food_ZoneFiltersDescription", "Only check players for food when in the following zones");
    }

    public class Sage
    {
        public readonly string Label = Loc.Localize("Sage_Label", "Sage");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Sage_ConfigurationPanelLabel", "Sage Configuration");
        public readonly string WarningText = Loc.Localize("Sage_WarningText", "Kardia Partner Missing");
        public readonly string Description = Loc.Localize("Sage_Description", "Sages use a job action commonly referred to as 'Kardia' or 'Kardion' on another party member to heal that party member whenever the sage uses a offensive ability.");
        public readonly string TechnicalDescription = Loc.Localize("Sage_TechnicalInformation", "This module will alert you when there are sages that do not have the 'Kardia' effect on themselves. This effect is granted to the sage when they also put 'Kardion' on another player.");
    }

    public class Scholar
    {
        public readonly string Label = Loc.Localize("Scholar_Label", "Scholar");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Scholar_ConfigurationPanelLabel", "Scholar Configuration");
        public readonly string WarningText = Loc.Localize("Scholar_WarningText", "Faerie Summon Missing");
        public readonly string Description = Loc.Localize("Scholar_Description", "Scholar uses a summoned pet known as a 'Faerie' to allow them to use various healing abilities. The faerie will periodically heal allies within its range automatically. A Scholar's ability to heal their party is significantly impacted if they do not summon their faerie.");
        public readonly string TechnicalDescription = Loc.Localize("Scholar_TechnicalInformation", "This module will alert you when a scholar does not have their faerie summoned, as long as that scholar does not also have the 'Dissipation' status effect.");
    }

    public class Summoner
    {
        public readonly string Label = Loc.Localize("Summoner_Label", "Summoner");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Summoner_ConfigurationPanelLabel", "Summoner Configuration");
        public readonly string WarningText = Loc.Localize("Summoner_WarningText", "Pet Summon Missing");
        public readonly string Description = Loc.Localize("Summoner_Description", "Summoners use a summoned pet known as a 'Carbuncle' to allow them to use various offensive abilities. The summoned pet will be transformed into various other summons during combat.");
        public readonly string TechnicalDescription = Loc.Localize("Summoner_TechnicalInformation", "This module will alert you when a summoner does not have any associated summon in the field.");
    }

    public class BlueMage
    {
        public readonly string Label = Loc.Localize("BlueMage_Label", "Blue Mage");
        public readonly string ConfigurationPanelLabel = Loc.Localize("BlueMage_ConfigurationPanelLabel", "Blue Mage Configuration");
        public readonly string WarningText = Loc.Localize("BlueMage_WarningText", "Blue Mage Action Missing");
        public readonly string Description = Loc.Localize("BlueMage_Description", "Blue Mage Stuff Here");
        public readonly string TechnicalDescription = Loc.Localize("BlueMage_TechnicalInformation", "Technical Blue Stuff Here");

    }
    #endregion

    #region Common
    public class Common
    {
        public Tabs Tabs { get; set; } = new();
        public Labels Labels { get; set; } = new();
    }

    public class Labels
    {
        public readonly string About = Loc.Localize("Labels_About", "About");
        public readonly string Options = Loc.Localize("Labels_Options", "Options");
        public readonly string Description = Loc.Localize("Labels_Description", "Description");
        public readonly string TechnicalDescription = Loc.Localize("Labels_TechnicalDescription", "Technical Description");
        public readonly string Priority = Loc.Localize("Labels_Priority", "Priority");
        public readonly string Seconds = Loc.Localize("Labels_Seconds", "Seconds");
        public readonly string City = Loc.Localize("Labels_City", "City");
        public readonly string OpenWorld = Loc.Localize("Labels_OpenWorld", "Open World");
        public readonly string Inn = Loc.Localize("Labels_Inn", "Inn");
        public readonly string Dungeon = Loc.Localize("Labels_Dungeon", "Dungeon");
        public readonly string AllianceRaid = Loc.Localize("Labels_AllianceRaid", "Alliance Raid");
        public readonly string Trial = Loc.Localize("Labels_Trial", "Trial");
        public readonly string Housing = Loc.Localize("Labels_Housing", "Housing");
        public readonly string Raid = Loc.Localize("Labels_Raid", "Raid");
        public readonly string GrandCompany = Loc.Localize("Labels_GrandCompany", "Grand Company");
        public readonly string DeepDiveDungeon = Loc.Localize("Labels_DeepDiveDungeon", "PotD/HoH");
        public readonly string Eureka = Loc.Localize("Labels_Eureka", "Eureka");
        public readonly string Bozja = Loc.Localize("Labels_Bozja", "Bozja");
        public readonly string Unknown = Loc.Localize("Labels_Unknown", "Unknown");
        public readonly string AdditionalOptions = Loc.Localize("Labels_AdditionalOptions", "Additional Options");
        public readonly string Savage = Loc.Localize("Labels_Savage", "Savage");
        public readonly string Ultimate = Loc.Localize("Labels_Ultimate", "Ultimate");
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
        public Blacklist Blacklist { get; set; } = new();
    }

    public class PartyOverlay
    {
        public readonly string Label = Loc.Localize("PartyOverlay_Label", "Party Frame Overlay");
        public readonly string Description = Loc.Localize("PartyOverlay_Description", "The party frame overlay I'll just write this later, if I forgot let me know.");
        public readonly string TechnicalDescription = Loc.Localize("PartyOverlay_TechnicalDescription", "Technical words, if I forgot this too, also let me know.");
        public readonly string DisplayOptions = Loc.Localize("PartyOverlay_DisplayOptions", "Display Options");
        public readonly string JobIcon = Loc.Localize("PartyOverlay_JobIcons", "Job Icon");
        public readonly string PlayerName = Loc.Localize("PartyOverlay_PlayerName", "Player Name");
        public readonly string WarningText = Loc.Localize("PartyOverlay_WarningText", "Warning Text");
        public readonly string FlashingEffects = Loc.Localize("PartyOverlay_FlashingEffects", "Flashing Effects");
    }

    public class Blacklist
    {
        public readonly string Label = Loc.Localize("Blacklist_Label", "Blacklist");
        public readonly string CurrentStatus = Loc.Localize("Blacklist_CurrentStatus", "Blacklisted Zones");
        public readonly string Empty = Loc.Localize("Blacklist_Empty", "Blacklist is empty");
        public readonly string Here = Loc.Localize("Blacklist_Here", "Add Here");
        public readonly string CurrentLocation = Loc.Localize("Blacklist_CurrentLocation", "Currently in {0}");
        public readonly string ID = Loc.Localize("Blacklist_ID", "Add by zone ID");
        public readonly string Name = Loc.Localize("Blacklist_Name", "Add by zone name");
    }
    #endregion

    #region Configuration
    public class Configuration
    {
        public readonly string NoSelection = Loc.Localize("Configuration_NoSelection", "Select an item to configure in the left pane");
        public readonly string Enable = Loc.Localize("Configuration_Enable", "Enable");
        public readonly string SoloMode = Loc.Localize("Configuration_SoloMode", "Solo Mode");
        public readonly string DutiesOnly = Loc.Localize("Configuration_DutiesOnly", "Duties Only");
        public readonly string DutiesOnlyHelp = Loc.Localize("Configuration_DutiesOnlyHelp", "When enabled will only show warnings while you are in a duty\nWhen disabled will show warnings everywhere");
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
        public readonly string Add = Loc.Localize("Commands_Add", "Add");
        public readonly string Remove = Loc.Localize("Commands_Remove", "Remove");
    }
    #endregion

    internal static class Strings
    {
        public static Modules Modules { get; set; } = new();
        public static Common Common { get; set; } = new();
        public static Configuration Configuration { get; set; } = new();
        public static Commands Commands { get; set; } = new();
        public static TabItems TabItems { get; set; } = new();

        public static void ReInitialize()
        {
            Modules = new();
            Common = new();
            Configuration = new();
            Commands = new();
            TabItems = new TabItems();
        }
    }
}
