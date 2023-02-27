using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Dalamud.Configuration;
using Dalamud.Logging;
using Newtonsoft.Json;
using NoTankYou.DataModels;
using NoTankYou.Modules;
using NoTankYou.UserInterface.Components;

namespace NoTankYou.Configuration;

[Serializable]
public class CharacterConfiguration : IPluginConfiguration
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
    public ChocoboConfiguration Chocobo = new();

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

    public static CharacterConfiguration Load(ulong contentID)
    {
        try
        {
            var configFileInfo = GetConfigFileInfo(contentID);

            if (TryLoadSpecificConfiguration(configFileInfo, out var loadedConfig))
            {
                return loadedConfig;
            }

            return CreateNewCharacterConfiguration();
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, $"Exception Occured during loading Character {contentID}. Loading new default config instead.");
            return CreateNewCharacterConfiguration();
        }
    }
    
    private static bool TryLoadSpecificConfiguration(FileSystemInfo? fileInfo, [NotNullWhen(true)] out CharacterConfiguration? info)
    {
        if (fileInfo is null || !fileInfo.Exists)
        {
            info = null;
            return false;
        }
        
        info = JsonConvert.DeserializeObject<CharacterConfiguration>(LoadFile(fileInfo));
        return info is not null;
    }
    
    private static FileInfo GetConfigFileInfo(ulong contentId) => new(Service.PluginInterface.ConfigDirectory.FullName + $@"\{contentId}.json");

    private static string LoadFile(FileSystemInfo fileInfo)
    {
        using var reader = new StreamReader(fileInfo.FullName);
        return reader.ReadToEnd();
    }

    private static void SaveFile(FileSystemInfo file, string fileText)
    {
        using var writer = new StreamWriter(file.FullName);
        writer.Write(fileText);
    }

    private void SaveConfigFile(FileSystemInfo file)
    {
        var text = JsonConvert.SerializeObject(this, Formatting.Indented);
        SaveFile(file, text);
    }
    
    private static CharacterConfiguration CreateNewCharacterConfiguration() => new()
    {
        CharacterData = new CharacterData
        {
            Name = Service.ClientState.LocalPlayer?.Name.TextValue ?? "Unknown",
            LocalContentID = Service.ClientState.LocalContentId,
        },
    };
}