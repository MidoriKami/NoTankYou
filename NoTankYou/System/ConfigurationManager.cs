using System;
using System.Diagnostics.CodeAnalysis;
using Dalamud.Game;
using Dalamud.Logging;
using NoTankYou.Configuration;

namespace NoTankYou.System;

public class ConfigurationManager : IDisposable
{
    private readonly CharacterConfiguration NullCharacterConfiguration = new();

    private CharacterConfiguration? BackingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? BackingCharacterConfiguration : NullCharacterConfiguration;

    private bool LoggedIn => Service.ClientState.LocalPlayer != null && Service.ClientState.LocalContentId != 0;
    
    [MemberNotNullWhen(returnValue: true, nameof(BackingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataAvailable;

    public ConfigurationManager()
    {
        if (LoggedIn)
        {
            PluginLog.Debug("Plugin was loaded while already logged in");
            LoadCharacterConfiguration();
        }

        PluginLog.Debug("Adding Login/Logout Listeners");
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
    }

    public void Dispose()
    {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
    }

    private void OnLogin(object? sender, EventArgs e)
    {
        PluginLog.Debug("Adding LoginLogic Listener");
        Service.Framework.Update += LoginLogic;
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        CharacterDataLoaded = false;
    }

    private void LoginLogic(Framework framework)
    {
        PluginLog.Debug($"LoggedIn: {LoggedIn}, LocalContentID:{Service.ClientState.LocalContentId}, LocalPlayer:{Service.ClientState.LocalPlayer?.Name ?? "Unknown"}");

        if (!LoggedIn) return;

        PluginLog.Debug("Character Data valid! Queuing Character load, Removing Listener");

        Service.Framework.RunOnTick(LoadCharacterConfiguration, TimeSpan.FromSeconds(1));
        Service.Framework.Update -= LoginLogic;
    }

    private void LoadCharacterConfiguration()
    {
        PluginLog.Debug("Loading Character Data");
        PluginLog.Debug($"LoggedIn: {LoggedIn}, LocalContentID:{Service.ClientState.LocalContentId}, LocalPlayer:{Service.ClientState.LocalPlayer?.Name ?? "Unknown"}");

        BackingCharacterConfiguration = CharacterConfiguration.Load(Service.ClientState.LocalContentId);
        CharacterDataLoaded = true;

        OnCharacterDataAvailable?.Invoke(this, CharacterConfiguration);
    }

    public void Save()
    {
        CharacterConfiguration.Save();
    }
}