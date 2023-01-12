using System;
using System.Diagnostics.CodeAnalysis;
using NoTankYou.Configuration;

namespace NoTankYou.System;

public class ConfigurationManager : IDisposable
{
    private readonly CharacterConfiguration nullCharacterConfiguration = new();

    private CharacterConfiguration? backingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? backingCharacterConfiguration : nullCharacterConfiguration;

    private static bool LoggedIn => Service.ClientState is { LocalPlayer: not null, LocalContentId: not 0 };
    
    [MemberNotNullWhen(returnValue: true, nameof(backingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataAvailable;

    public ConfigurationManager()
    {
        if (LoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
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
        backingCharacterConfiguration = CharacterConfiguration.Load(Service.ClientState.LocalContentId);
        CharacterDataLoaded = true;

        OnCharacterDataAvailable?.Invoke(this, CharacterConfiguration);
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        CharacterDataLoaded = false;
    }

    public void Save()
    {
        CharacterConfiguration.Save();
    }
}