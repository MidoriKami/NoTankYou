using System;
using System.IO;
using Dalamud.Logging;
using Newtonsoft.Json;
using NoTankYou.DataModels;
using NoTankYou.Modules;
using NoTankYou.UserInterface.Windows;

namespace NoTankYou.Configuration;

[Serializable]
public class CharacterConfiguration
{
    public int Version { get; set; } = 5;

    public CharacterData CharacterData = new();

    public BlueMageConfiguration BlueMage = new();
    public DancerConfiguration Dancer = new();
    public TankConfiguration Tank = new();
    public FoodConfiguration Food = new();
    public FreeCompanyConfiguration FreeCompany = new();
    public SageConfiguration Sage = new();
    public ScholarConfiguration Scholar = new();
    public SummonerConfiguration Summoner = new();
    public SpiritBondConfiguration SpiritBond = new();
    public CutsceneConfiguration Cutscene = new();

    public PartyOverlaySettings PartyOverlay = new();
    public BannerOverlaySettings BannerOverlay = new();
    public BlacklistSettings Blacklist = new();

    public void Save()
    {
        if (CharacterData.LocalContentID != 0)
        {
            var configFileInfo = GetConfigFileInfo(CharacterData.LocalContentID);

            var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

            var writer = new StreamWriter(configFileInfo.FullName);
            writer.Write(serializedContents);
            writer.Dispose();
        }
    }

    private static FileInfo GetConfigFileInfo(ulong contentID)
    {
        var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.json");
    }
    
    public static CharacterConfiguration Load(ulong contentID)
    {
        // If a configuration for this character already exists
        if ( GetConfigFileInfo(contentID) is { Exists: true } configFileInfo )
        {
            return LoadExistingCharacterConfiguration(contentID, configFileInfo);
        }

        // If a configuration for this character doesn't exist
        else
        {
            var basePluginConfigInfo = Service.PluginInterface.ConfigFile;

            // If a base-plugin config exists
            if (basePluginConfigInfo.Exists)
            {
                return GenerateMigratedCharacterConfiguration(basePluginConfigInfo);
            }
            // If it doesn't make a new config for this character
            else
            {
                return CreateNewCharacterConfiguration();
            }
        }
    }

    private static CharacterConfiguration GenerateMigratedCharacterConfiguration(FileInfo basePluginConfigInfo)
    {
        CharacterConfiguration migratedConfiguration;

        try
        {
            migratedConfiguration = ConfigMigration.Convert(basePluginConfigInfo);
            migratedConfiguration.Save();
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, "Unable to Migrate Configuration, generating new configuration instead.");
            migratedConfiguration = CreateNewCharacterConfiguration();
        }

        // The user may have saved values here that we want to ignore
        migratedConfiguration.BlueMage.DutiesOnly.Value = false;
        migratedConfiguration.BlueMage.SoloMode.Value = false;
        
        return migratedConfiguration;
    }

    private static CharacterConfiguration LoadExistingCharacterConfiguration(ulong contentID, FileInfo configFileInfo)
    {
        var reader = new StreamReader(configFileInfo.FullName);
        var fileText = reader.ReadToEnd();
        reader.Dispose();

        var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

        if (loadedCharacterConfiguration == null)
        {
            throw new FileLoadException($"Unable to load configuration file for contentID: {contentID}");
        }

        // The user may have saved values here that we want to ignore
        loadedCharacterConfiguration.BlueMage.DutiesOnly.Value = false;
        loadedCharacterConfiguration.BlueMage.SoloMode.Value = false;
        
        return loadedCharacterConfiguration;
    }

    private static CharacterConfiguration CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new CharacterConfiguration();

        var playerData = Service.ClientState.LocalPlayer;
        var contentId = Service.ClientState.LocalContentId;

        var playerName = playerData?.Name.TextValue ?? "Unknown";

        newCharacterConfiguration.CharacterData = new CharacterData()
        {
            Name = playerName,
            LocalContentID = contentId,
        };

        newCharacterConfiguration.Save();
        return newCharacterConfiguration;
    }
}