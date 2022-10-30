using CheapLoc;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace NoTankYou.Localization;

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
    public SpiritBond SpiritBond { get; set; } = new();
    public Cutscene Cutscene { get; set; } = new();
}

public class Tank
{
    public string Label => Loc.Localize("Tank_Label", "Tanks");
    public string WarningText => Loc.Localize("Tank_WarningText", "Tank Stance Missing");
    public string WarningTextShort => Loc.Localize("Tank_WarningTextShort", "Tank Stance");
    public string DisableInAllianceRaid => Loc.Localize("Tank_DisableInAllianceRaid", "Disable in Alliance Raid");
    public string CheckAllianceStances => Loc.Localize("Tank_CheckAllianceStances", "Check Alliance Members");
}

public class FreeCompany
{
    public string Label => Loc.Localize("FreeCompany_Label", "Free Company");
    public string WarningText => Loc.Localize("FreeCompany_WarningText", "FreeCompany Buff Missing");
    public string WarningTextShort => Loc.Localize("FreeCompany_WarningTextShort", "FreeCompany Buff");
    public string Any => Loc.Localize("FreeCompany_Any", "Any");
    public string Specific => Loc.Localize("FreeCompany_Specific", "Specific");
    public string BuffCount => Loc.Localize("FreeCompany_BuffCount", "Buff Count");
    public string BuffSelection => Loc.Localize("FreeCompany_BuffSelection", "Buff Selection");
}

public class Dancer
{
    public string Label => Loc.Localize("Dancer_Label", "Dancer");
    public string WarningText => Loc.Localize("Dancer_WarningText", "Dance Partner Missing");
    public string WarningTextShort => Loc.Localize("Dancer_WarningTextShort", "Dance Partner");
}

public class Food
{
    public string EnableFilter => Loc.Localize("Food_EnableFilter", "Enable Filter");
    public string Label => Loc.Localize("Food_Label", "Food");
    public string WarningText => Loc.Localize("Food_WarningText", "Food Missing");
    public string EarlyWarningLabel => Loc.Localize("Food_EarlyWarningLabel", "Early Warning Time");
    public string ZoneFilters => Loc.Localize("Food_ZoneFilters", "Zone Filters");
    public string ZoneFiltersDescription => Loc.Localize("Food_ZoneFiltersDescription", "Display warning only in the following zones");
    public string AdditionalOptionsLabel => Loc.Localize("Food_AdditionalOptionsLabel", "Additional Options");
    public string SuppressInCombat => Loc.Localize("Food_SuppressInCombat", "Suppress in Combat");
}

public class Sage
{
    public string Label => Loc.Localize("Sage_Label", "Sage");
    public string WarningText => Loc.Localize("Sage_WarningText", "Kardia Partner Missing");
    public string WarningTextShort => Loc.Localize("Sage_WarningTextShort", "Kardia Partner");
}

public class Scholar
{
    public string Label => Loc.Localize("Scholar_Label", "Scholar");
    public string WarningText => Loc.Localize("Scholar_WarningText", "Faerie Summon Missing");
    public string WarningTextShort => Loc.Localize("Scholar_WarningTextShort", "Faerie Summon");
}

public class Summoner
{
    public string Label => Loc.Localize("Summoner_Label", "Summoner");
    public string WarningText => Loc.Localize("Summoner_WarningText", "Pet Summon Missing");
    public string WarningTextShort => Loc.Localize("Summoner_WarningTextShort", "Pet Summon");
}

public class BlueMage
{
    public string Label => Loc.Localize("BlueMage_Label", "Blue Mage");
    public string Mimicry => Loc.Localize("BlueMage_Mimicry", "Aetherial Mimicry Missing");
    public string MightyGuard => Loc.Localize("BlueMage_MightyGuard", "Mighty Guard Missing");
    public string BasicInstinct => Loc.Localize("BlueMage_BasicInstinct", "Basic Instinct Missing");
    public string MimicryLabel => Loc.Localize("BlueMage_MimicryLabel", "Aetherial Mimicry");
    public string MightyGuardLabel => Loc.Localize("BlueMage_MightyGuardLabel", "Mighty Guard");
    public string BasicInstinctLabel => Loc.Localize("BlueMage_BasicInstinctLabel", "Basic Instinct");
}

public class SpiritBond
{
    public string EnableFilter => Loc.Localize("SpiritBond_EnableFilter", "Enable Filter");
    public string Label => Loc.Localize("SpiritBond_Label", "Spiritbond");
    public string WarningText => Loc.Localize("SpiritBond_WarningText", "Spiritbond Missing");
    public string EarlyWarningLabel => Loc.Localize("SpiritBond_EarlyWarningLabel", "Early Warning Time");
    public string ZoneFilters => Loc.Localize("SpiritBond_ZoneFilters", "Zone Filters");
    public string ZoneFiltersDescription => Loc.Localize("SpiritBond_ZoneFiltersDescription", "Display warning only in the following zones");
    public string AdditionalOptionsLabel => Loc.Localize("SpiritBond_AdditionalOptionsLabel", "Additional Options");
    public string SuppressInCombat => Loc.Localize("SpiritBond_SuppressInCombat", "Suppress in Combat");
}

public class Cutscene
{
    public string Label => Loc.Localize("Cutscene_Label", "Cutscene");
    public string CheckAlliance => Loc.Localize("Cutscene_CheckAlliance", "Check Alliance Members");
    public string WarningText => Loc.Localize("Cutscene_WarningText", "In Cutscene");
    public string WarningTextShort => Loc.Localize("Cutscene_WarningText", "In Cutscene");
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
    public string Enabled => Loc.Localize("Labels_Enabled", "Enabled");
    public string Disabled => Loc.Localize("Labels_Disabled", "Disabled");
    public string Options => Loc.Localize("Labels_Options", "Options");
    public string Priority => Loc.Localize("Labels_Priority", "Priority");
    public string Seconds => Loc.Localize("Labels_Seconds", "Seconds");
    public string City => Loc.Localize("Labels_City", "City");
    public string OpenWorld => Loc.Localize("Labels_OpenWorld", "Open World");
    public string Inn => Loc.Localize("Labels_Inn", "Inn");
    public string Dungeon => Loc.Localize("Labels_Dungeon", "Dungeon");
    public string AllianceRaid => Loc.Localize("Labels_AllianceRaid", "Alliance Raid");
    public string Trial => Loc.Localize("Labels_Trial", "Trial");
    public string Housing => Loc.Localize("Labels_Housing", "Housing");
    public string Raid => Loc.Localize("Labels_Raid", "Raid");
    public string GrandCompany => Loc.Localize("Labels_GrandCompany", "Grand Company");
    public string DeepDiveDungeon => Loc.Localize("Labels_DeepDiveDungeon", "PotD/HoH");
    public string Eureka => Loc.Localize("Labels_Eureka", "Eureka");
    public string Bozja => Loc.Localize("Labels_Bozja", "Bozja");
    public string Unknown => Loc.Localize("Labels_Unknown", "Unknown");
    public string AdditionalOptions => Loc.Localize("Labels_AdditionalOptions", "Additional Options");
    public string Savage => Loc.Localize("Labels_Savage", "Savage");
    public string Ultimate => Loc.Localize("Labels_Ultimate", "Ultimate");
    public string ExtremeUnreal => Loc.Localize("Labels_ExtremeUnreal", "Extreme & Unreal");
    public string Criterion => Loc.Localize("Labels_Criterion", "Criterion");
    public string ModeSelect => Loc.Localize("Labels_ModeSelect", "Mode Select");
    public string Unset => Loc.Localize("Labels_Unset", "Unset");
    public string Warnings => Loc.Localize("Labels_Warnings", "Warnings");
    public string DisplayOptions => Loc.Localize("Labels_DisplayOptions", "Display Options");
    public string Scale => Loc.Localize("Labels_Scale", "Scale");
}

public class Tabs
{
    public string Settings => Loc.Localize("Tabs_Settings", "Settings");
}

public class TabItems
{
    public PartyOverlay PartyOverlay { get; set; } = new();
    public Blacklist Blacklist { get; set; } = new();
    public BannerOverlay BannerOverlay { get; set; } = new();
}

public class BannerOverlay
{
    public string BorderThickness => Loc.Localize("BannerOverlay_BorderThickness", "Border Thickness");
    public string Label => Loc.Localize("BannerOverlay_Label", "Banner Overlay");
    public string ConfigurationLabel => Loc.Localize("BannerOverlay_ConfigurationLabel", "Banner Overlay Configuration");
    public string WarningCount => Loc.Localize("BannerOverlay_WarningCount", "Player Count");
    public string ListModeDescription => Loc.Localize("BannerOverlay_ListModeDescription", "Displays warnings from multiple party members at the same time");
    public string TopPriorityDescription => Loc.Localize("BannerOverlay_TopPriorityDescription", "Display only one warning at a time");
    public string ListMode => Loc.Localize("BannerOverlay_ListMode", "List");
    public string TopPriorityMode => Loc.Localize("BannerOverlay_TopPriorityMode", "Single");
    public string ListModeOptions => Loc.Localize("BannerOverlay_ListModeOptions", "List Mode Options");
    public string RepositionMode => Loc.Localize("BannerOverlay_RepositionMode", "Sample Mode - Allows repositioning");
    public string ExclamationMark => Loc.Localize("BannerOverlay_ExclamationMark", "Show Exclamation Mark");
    public string WarningText => Loc.Localize("BannerOverlay_WarningText", "Show Text");
    public string Icon => Loc.Localize("BannerOverlay_Icon", "Show Icon");
    public string PlayerNames => Loc.Localize("BannerOverlay_PlayerNames", "Show Player Names");
    public string IconText => Loc.Localize("BannerOverlay_IconText", "Show Icon Text");
    public string Button => Loc.Localize("BannerOverlay_Button", "Banner Overlay");
    public string SoloMode => Loc.Localize("BannerOverlay_SoloMode", "Solo Mode");
    public string SoloModeHelp => Loc.Localize("BannerOverlay_SoloModeHelp", "Only display Warnings that are generated by you");
}

public class PartyOverlay
{
    public string Label => Loc.Localize("PartyOverlay_Label", "Party Frame Overlay");
    public string ConfigurationLabel => Loc.Localize("PartyOverlay_ConfigurationLabel", "Party Frame Overlay Configuration");
    public string JobIcon => Loc.Localize("PartyOverlay_JobIcons", "Job Icon");
    public string PlayerName => Loc.Localize("PartyOverlay_PlayerName", "Player Name");
    public string WarningText => Loc.Localize("PartyOverlay_WarningText", "Warning Text");
    public string FlashingEffects => Loc.Localize("PartyOverlay_FlashingEffects", "Flashing Effects");
    public string ColorOptions => Loc.Localize("PartyOverlay_ColorOptions", "Color Options");
    public string WarningOutlineColor => Loc.Localize("PartyOverlay_WarningOutlineColor", "Name Outline");
    public string Button => Loc.Localize("PartyOverlay_Button", "Party Frame Overlay");
}

public class Blacklist
{
    public string Label => Loc.Localize("Blacklist_Label", "Blacklist");
    public string Button => Loc.Localize("Blacklist_Button", "Blacklist Settings");
    public string CurrentStatus => Loc.Localize("Blacklist_CurrentStatus", "Blacklisted Zones");
    public string Empty => Loc.Localize("Blacklist_Empty", "Blacklist is empty");
    public string Here => Loc.Localize("Blacklist_Here", "Add Here");
    public string CurrentLocation => Loc.Localize("Blacklist_CurrentLocation", "Currently in {0}");
    public string ID => Loc.Localize("Blacklist_ID", "Add by zone ID");
    public string Name => Loc.Localize("Blacklist_Name", "Add by zone name");
}
#endregion

#region Configuration
public class Configuration
{
    public string NoSelection => Loc.Localize("Configuration_NoSelection", "Select an item to configure in the left pane");
    public string Enable => Loc.Localize("Configuration_Enable", "Enable");
    public string SoloMode => Loc.Localize("Configuration_SoloMode", "Solo Mode");
    public string DutiesOnly => Loc.Localize("Configuration_DutiesOnly", "Duties Only");
    public string HideInSanctuary => Loc.Localize("Configuration_HideInSanctuary", "Hide in Sanctuaries");
    public string DutiesOnlyHelp => Loc.Localize("Configuration_DutiesOnlyHelp", "When enabled will only show warnings while you are in a duty\nWhen disabled will show warnings everywhere");
    public string SoloModeHelp => Loc.Localize("Configuration_SoloModeHelp", "When enabled will only evaluate warnings for you");
    public string PreviewMode => Loc.Localize("Configuration_PreviewMode", "Sample Mode");
}
#endregion

#region Commands
public class Commands
{
    public string Add => Loc.Localize("Commands_Add", "Add");
    public string Remove => Loc.Localize("Commands_Remove", "Remove");
}
#endregion

internal static class Strings
{
    public static Modules Modules { get; set; } = new();
    public static Common Common { get; set; } = new();
    public static Configuration Configuration { get; set; } = new();
    public static Commands Commands { get; set; } = new();
    public static TabItems TabItems { get; set; } = new();
}