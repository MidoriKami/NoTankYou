namespace NoTankYou.Utilities;

/// <summary>
/// Configuration File Utilities
/// </summary>
public static class Config {
    public static string CharacterConfigPath => FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath()).FullName;

    /// <summary>
    /// Loads a character specific config file from PluginConfigs\NoTankYou\{ContentId}\{FileName}
    /// Creates a `new T()` or uses passed in defaultValue object if the file can't be loaded
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static T LoadCharacterConfig<T>(string fileName, T? defaultValue = null) where T : class, new()
        => FileHelpers.LoadFile(FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName, defaultValue);
    
    /// <summary>
    /// Saves a character specific config file to PluginConfigs\NoTankYou\{ContentId}\{FileName}
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static void SaveCharacterConfig<T>(T modificationConfig, string fileName)
        => FileHelpers.SaveFile(modificationConfig, FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);
}

