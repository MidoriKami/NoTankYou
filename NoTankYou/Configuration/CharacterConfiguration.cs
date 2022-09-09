using System;
using System.IO;
using Dalamud.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NoTankYou.Configuration.Components;
using NoTankYou.Configuration.ModuleSettings;
using NoTankYou.Configuration.Overlays;
using NoTankYou.Utilities;

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
        var configFileInfo = GetConfigFileInfo(contentID);

        // If a configuration for this character already exists
        if (configFileInfo.Exists)
        {
            var reader = new StreamReader(configFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Dispose();

            var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

            if (loadedCharacterConfiguration == null)
            {
                throw new FileLoadException($"Unable to load configuration file for contentID: {contentID}");
            }

            return loadedCharacterConfiguration;
        }

        // If a configuration for this character doesn't exist
        else
        {
            var basePluginConfigInfo = Service.PluginInterface.ConfigFile;

            // If a base-plugin config exists
            if (basePluginConfigInfo.Exists)
            {
                var reader = new StreamReader(basePluginConfigInfo.FullName);
                var fileText = reader.ReadToEnd();
                reader.Dispose();

                CharacterConfiguration migratedConfiguration;

                try
                {
                    migratedConfiguration = ConfigMigration.Convert(fileText);
                    migratedConfiguration.Save();
                }
                catch (Exception e)
                {
                    PluginLog.Warning(e, "Unable to Migrate Configuration, generating new configuration instead.");
                    migratedConfiguration = CreateNewCharacterConfiguration();
                }

                return migratedConfiguration;
            }

            // If it doesn't make a new config for this character
            else
            {
                return CreateNewCharacterConfiguration();
            }
        }
    }

    private static CharacterConfiguration CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new CharacterConfiguration();

        var playerData = Service.ClientState.LocalPlayer;
        var contentId = Service.ClientState.LocalContentId;

        var playerName = playerData?.Name.TextValue ?? "Unknown";
        var playerWorld = playerData?.HomeWorld.GameData?.Name.ToString() ?? "UnknownWorld";

        newCharacterConfiguration.CharacterData = new CharacterData()
        {
            Name = playerName,
            LocalContentID = contentId,
            World = playerWorld,
        };

        newCharacterConfiguration.Save();
        return newCharacterConfiguration;
    }

    private static int GetConfigFileVersion(string fileText)
    {
        var json = JObject.Parse(fileText);

        return json.GetValue("Version")?.Value<int>() ?? 0;
    }
}