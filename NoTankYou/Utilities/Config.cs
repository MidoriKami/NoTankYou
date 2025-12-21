namespace NoTankYou.Utilities;

/// <summary>
/// Configuration File Utilities
/// </summary>
public static class Config {
    public static string CharacterConfigPath => FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath()).FullName;

    /// <summary>
    /// Loads a character specific config file from PluginConfigs\NoTankYou\{ContentId}\{FileName}
    /// Creates a `new T` if the file can't be loaded
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static T LoadCharacterConfig<T>(string fileName) where T : new()
        => FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);
    
    /// <summary>
    /// Saves a character specific config file to PluginConfigs\NoTankYou\{ContentId}\{FileName}
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static void SaveCharacterConfig<T>(T modificationConfig, string fileName)
        => FileHelpers.SaveFile(modificationConfig, FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);
}

