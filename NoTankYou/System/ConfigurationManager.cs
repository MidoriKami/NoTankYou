using System;
using System.Diagnostics.CodeAnalysis;
using Dalamud.Game;
using NoTankYou.Configuration;

namespace NoTankYou.System;

public class ConfigurationManager : IDisposable
{
    private readonly CharacterConfiguration NullCharacterConfiguration = new();

    private CharacterConfiguration? BackingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? BackingCharacterConfiguration : NullCharacterConfiguration;

    private static bool LoggedIn => Service.ClientState is { LocalPlayer: not null, LocalContentId: not 0 };
    
    [MemberNotNullWhen(returnValue: true, nameof(BackingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataAvailable;

    public ConfigurationManager()
    {
        if (LoggedIn)
        {
            LoadCharacterConfiguration();
        }

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
        Service.Framework.Update += LoginLogic;
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        CharacterDataLoaded = false;
    }

    private void LoginLogic(Framework framework)
    {
        if (!LoggedIn) return;

        Service.Framework.RunOnTick(LoadCharacterConfiguration, TimeSpan.FromSeconds(1));
        Service.Framework.Update -= LoginLogic;
    }

    private void LoadCharacterConfiguration()
    {

        BackingCharacterConfiguration = CharacterConfiguration.Load(Service.ClientState.LocalContentId);
        CharacterDataLoaded = true;

        OnCharacterDataAvailable?.Invoke(this, CharacterConfiguration);
    }

    public void Save()
    {
        CharacterConfiguration.Save();
    }
}