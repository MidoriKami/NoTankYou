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
        public FreeCompany FreeCompany { get; set; } = new();
    }

    public class Tank
    {
        public readonly string Label = Loc.Localize("Tank_Label", "Tanks");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Tank_ConfigurationPanelLabel", "Tanks Configuration");
        public readonly string WarningText = Loc.Localize("Tank_WarningText", "Tank Stance Missing");
        public readonly string WarningTextShort = Loc.Localize("Tank_WarningTextShort", "Tank Stance");
        public readonly string DisableInAllianceRaid = Loc.Localize("Tank_DisableInAllianceRaid", "Disable in Alliance Raid");
        public readonly string CheckAllianceStances = Loc.Localize("Tank_CheckAllianceStances", "Check Alliance Members");
        public readonly string CheckAllianceStancesDescription = Loc.Localize("Tank_CheckAllianceStancesDescription", "If Enabled, checks other alliance's tanks for a tank stance to warn if no tanks have their stance on\nIf Disabled, only checks your party for tank stances");
    }

    public class FreeCompany
    {
        public readonly string Label = Loc.Localize("FreeCompany_Label", "Free Company");
        public readonly string ConfigurationPanelLabel = Loc.Localize("FreeCompany_ConfigurationPaneLabel", "Free Company Configuration");
        public readonly string WarningText = Loc.Localize("FreeCompany_WarningText", "FreeCompany Buff Missing");
        public readonly string WarningTextShort = Loc.Localize("FreeCompany_WarningTextShort", "FreeCompany Buff");
        public readonly string Any = Loc.Localize("FreeCompany_Any", "Any");
        public readonly string AnyDescription = Loc.Localize("FreeCompany_AnyDescription", "Display warning if missing any Free Company buffs");
        public readonly string Specific = Loc.Localize("FreeCompany_Specific", "Specific");
        public readonly string SpecificDescription = Loc.Localize("FreeCompany_SpecificDescription", "Display warning if missing specific Free Company buffs");
        public readonly string BuffCount = Loc.Localize("FreeCompany_BuffCount", "Buff Count");
        public readonly string BuffSelection = Loc.Localize("FreeCompany_BuffSelection", "Buff Selection");
    }

    public class Dancer
    {
        public readonly string Label = Loc.Localize("Dancer_Label", "Dancer");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Dancer_ConfigurationPanelLabel", "Dancer Configuration");
        public readonly string WarningText = Loc.Localize("Dancer_WarningText", "Dance Partner Missing");
        public readonly string WarningTextShort = Loc.Localize("Dancer_WarningTextShort", "Dance Partner");
    }

    public class Food
    {
        public readonly string Label = Loc.Localize("Food_Label", "Food");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Food_ConfigurationPanelLabel", "Food Configuration");
        public readonly string WarningText = Loc.Localize("Food_WarningText", "Food Missing");
        public readonly string EarlyWarningLabel = Loc.Localize("Food_EarlyWarningLabel", "Early Warning Time");
        public readonly string ZoneFilters = Loc.Localize("Food_ZoneFilters", "Zone Filters");
        public readonly string ZoneFiltersDescription = Loc.Localize("Food_ZoneFiltersDescription", "Only check players for food when in the following zones");
        public readonly string AdditionalOptionsLabel = Loc.Localize("Food_AdditionalOptionsLabel", "Additional Options");
        public readonly string SuppressInCombat = Loc.Localize("Food_SuppressInCombat", "Suppress in Combat");
    }

    public class Sage
    {
        public readonly string Label = Loc.Localize("Sage_Label", "Sage");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Sage_ConfigurationPanelLabel", "Sage Configuration");
        public readonly string WarningText = Loc.Localize("Sage_WarningText", "Kardia Partner Missing");
        public readonly string WarningTextShort = Loc.Localize("Sage_WarningTextShort", "Kardia Partner");
    }

    public class Scholar
    {
        public readonly string Label = Loc.Localize("Scholar_Label", "Scholar");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Scholar_ConfigurationPanelLabel", "Scholar Configuration");
        public readonly string WarningText = Loc.Localize("Scholar_WarningText", "Faerie Summon Missing");
        public readonly string WarningTextShort = Loc.Localize("Scholar_WarningTextShort", "Faerie Summon");
    }

    public class Summoner
    {
        public readonly string Label = Loc.Localize("Summoner_Label", "Summoner");
        public readonly string ConfigurationPanelLabel = Loc.Localize("Summoner_ConfigurationPanelLabel", "Summoner Configuration");
        public readonly string WarningText = Loc.Localize("Summoner_WarningText", "Pet Summon Missing");
        public readonly string WarningTextShort = Loc.Localize("Summoner_WarningTextShort", "Pet Summon");
    }

    public class BlueMage
    {
        public readonly string Label = Loc.Localize("BlueMage_Label", "Blue Mage");
        public readonly string ConfigurationPanelLabel = Loc.Localize("BlueMage_ConfigurationPanelLabel", "Blue Mage Configuration");
        public readonly string GenericWarning = Loc.Localize("BlueMage_GenericWarning", "Warning Something is Missing");
        public readonly string Mimicry = Loc.Localize("BlueMage_Mimicry", "Aetherial Mimicry Missing");
        public readonly string MightyGuard = Loc.Localize("BlueMage_MightyGuard", "Mighty Guard Missing");
        public readonly string BasicInstinct = Loc.Localize("BlueMage_BasicInstinct", "Basic Instinct Missing");
        public readonly string MimicryLabel = Loc.Localize("BlueMage_MimicryLabel", "Aetherial Mimicry");
        public readonly string MightyGuardLabel = Loc.Localize("BlueMage_MightyGuardLabel", "Mighty Guard");
        public readonly string BasicInstinctLabel = Loc.Localize("BlueMage_BasicInstinctLabel", "Basic Instinct");
        public readonly string MimicryWarning = Loc.Localize("BlueMage_MimicryWarning", "Can not be used with 'Duties Only'");
        public readonly string BasicInstinctInfo = Loc.Localize("BlueMage_BasicInstinctInfo", "Warning will only show when solo with no other players");
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
        public readonly string Options = Loc.Localize("Labels_Options", "Options");
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
        public readonly string ExtremeUnreal = Loc.Localize("Labels_ExtremeUnreal", "Extreme & Unreal");
        public readonly string ModeSelect = Loc.Localize("Labels_ModeSelect", "Mode Select");
        public readonly string Unset = Loc.Localize("Labels_Unset", "Unset");
        public readonly string Warnings = Loc.Localize("Labels_Warnings", "Warnings");
        public readonly string DisplayOptions = Loc.Localize("Labels_DisplayOptions", "Display Options");
        public readonly string Scale = Loc.Localize("Labels_Scale", "Scale");
        public readonly string Locked = Loc.Localize("Labels_Locked", "Locked");
        public readonly string Lock = Loc.Localize("Labels_Lock", "Lock");
        public readonly string Unlocked = Loc.Localize("Labels_Unlocked", "Unlocked");
        public readonly string Unlock = Loc.Localize("Labels_Unlock", "Unlock");
        public readonly string Apply = Loc.Localize("Labels_Apply", "Apply");
        public readonly string Reset = Loc.Localize("Labels_Reset", "Reset");
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
        public Attributions Attributions { get; set; } = new();
        public BannerOverlay BannerOverlay { get; set; } = new();
        public AdvancedOptions AdvancedOptions { get; set; } = new();
    }

    public class BannerOverlay
    {
        public readonly string Label = Loc.Localize("BannerOverlay_Label", "Banner Overlay");
        public readonly string ConfigurationLabel = Loc.Localize("BannerOverlay_ConfigurationLabel", "Warning Banner Overlay Configuration");
        public readonly string WarningCountDescription = Loc.Localize("BannerOverlay_WarningCountDescription", "Maximum number of players to display");
        public readonly string WarningCount = Loc.Localize("BannerOverlay_WarningCount", "Player Count");
        public readonly string TopPriorityDescription = Loc.Localize("BannerOverlay_TopPriorityDescription", "Display only one warning at a time");
        public readonly string ListMode = Loc.Localize("BannerOverlay_ListMode", "List");
        public readonly string ListModeDescription = Loc.Localize("BannerOverlay_ListModeDescription", "Displays warnings from multiple party members at the same time");
        public readonly string TopPriorityMode = Loc.Localize("BannerOverlay_TopPriorityMode", "Single");
        public readonly string ListModeOptions = Loc.Localize("BannerOverlay_ListModeOptions", "List Mode Options");
        public readonly string RepositionMode = Loc.Localize("BannerOverlay_RepositionMode", "Repositioning");
        public readonly string ExclamationMark = Loc.Localize("BannerOverlay_ExclamationMark", "Show Exclamation Mark");
        public readonly string WarningText = Loc.Localize("BannerOverlay_WarningText", "Show Text");
        public readonly string Icon = Loc.Localize("BannerOverlay_Icon", "Show Icon");
        public readonly string UnlockToSave = Loc.Localize("BannerOverlay_UnlockToSave", "Lock to Apply");
        public readonly string PlayerNames = Loc.Localize("BannerOverlay_PlayerNames", "Show Player Names");
        public readonly string IconText = Loc.Localize("BannerOverlay_IconText", "Show Icon Text");
    }

    public class PartyOverlay
    {
        public readonly string Label = Loc.Localize("PartyOverlay_Label", "Party Frame Overlay");
        public readonly string ConfigurationLabel = Loc.Localize("PartyOverlay_ConfigurationLabel", "Party Frame Overlay Configuration");
        public readonly string JobIcon = Loc.Localize("PartyOverlay_JobIcons", "Job Icon");
        public readonly string PlayerName = Loc.Localize("PartyOverlay_PlayerName", "Player Name");
        public readonly string WarningText = Loc.Localize("PartyOverlay_WarningText", "Warning Text");
        public readonly string FlashingEffects = Loc.Localize("PartyOverlay_FlashingEffects", "Flashing Effects");
        public readonly string ColorOptions = Loc.Localize("PartyOverlay_ColorOptions", "Color Options");
        public readonly string WarningOutlineColor = Loc.Localize("PartyOverlay_WarningOutlineColor", "Name Outline");
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

    public class Attributions
    {
        public readonly string Label = Loc.Localize("Attributions_Label", "Attributions");
        public readonly string FreeCompanyDescription = Loc.Localize("Attributions_FreeCompanyDescription", "Free Company Artwork by 'artofawang'");

    }

    public class AdvancedOptions
    {
        public readonly string Label = Loc.Localize("AdvancedOptions_Label", "Advanced Options");
        public readonly string DisablePartyListChecking = Loc.Localize("AdvancedOptions_DisablePartyListChecking", "Party List Visibility Checking");
        public readonly string DisablePartyListCheckingDisclaimer = Loc.Localize("AdvancedOptions_DisablePartyListCheckingDisclaimer", 
            "NoTankYou uses the visibility status of the Party List to know when to show warnings\n\n" +
            "If the party list is being hidden through other means, NoTankYou will not show any warnings\n\n" +
            "You can override this behavior by disabling visibility checking\n\n" +
            "When visibility checking is disabled, warnings will appear when they otherwise would not, such as during cutscenes, active time events, area transitions, and more");
        public readonly string StrongWarning = Loc.Localize("AdvancedOptions_StrongWarning", "Only use this setting as a last resort!");
        public readonly string DisableFeature = Loc.Localize("Configuration_DisableFeature", "Disable Visibility Checking");
    }
    #endregion

    #region Configuration
    public class Configuration
    {
        public readonly string NoSelection = Loc.Localize("Configuration_NoSelection", "Select an item to configure in the left pane");
        public readonly string Enable = Loc.Localize("Configuration_Enable", "Enable");
        public readonly string SoloMode = Loc.Localize("Configuration_SoloMode", "Solo Mode");
        public readonly string DutiesOnly = Loc.Localize("Configuration_DutiesOnly", "Duties Only");
        public readonly string HideInSanctuary = Loc.Localize("Configuration_HideInSanctuary", "Hide in Sanctuaries");
        public readonly string DutiesOnlyHelp = Loc.Localize("Configuration_DutiesOnlyHelp", "When enabled will only show warnings while you are in a duty\nWhen disabled will show warnings everywhere");
        public readonly string SoloModeHelp = Loc.Localize("Configuration_SoloModeHelp", "Only evaluates warnings for you, does not check party members\n\nRequires the party frame to be visible to evaluate warnings\nSystem > Character Configuration > UI Settings > Party List > (Uncheck) Hide party list while solo");
    }
    #endregion

    #region Commands
    public class Commands
    {
        public readonly string Add = Loc.Localize("Commands_Add", "Add");
        public readonly string Remove = Loc.Localize("Commands_Remove", "Remove");
        public readonly string Help = Loc.Localize("Commands_Help", "NoTankYou Commands:\n" +
                                                                    "/nty blu (on | off) - Toggle Blue Mage Module\n" +
                                                                    "/nty dnc (on | off) - Toggle Dancer Module\n" +
                                                                    "/nty food (on | off) - Toggle Food Module\n" +
                                                                    "/nty fc (on | off) - Toggle Free Company Module\n" +
                                                                    "/nty sge (on | off) - Toggle Sage Module\n" +
                                                                    "/nty sch (on | off) - Toggle Scholar Module\n" +
                                                                    "/nty smn (on | off) - Toggle Summoner Module\n" +
                                                                    "/nty tanks (on | off) - Toggle Tanks Module\n" +
                                                                    "/nty partyoverlay (on | off) - Toggle Party Overlay\n" +
                                                                    "/nty banneroverlay (on | off) - Toggle Banner Overlay");
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
