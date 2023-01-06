using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using KamiLib.BlacklistSystem;
using KamiLib.InfoBoxSystem;
using NoTankYou.Configuration;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Windows;

public class BlacklistConfigurationWindow : Window, IDisposable
{
    private static BlacklistSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Blacklist;
    
    public BlacklistConfigurationWindow() : base($"{Strings.Blacklist_Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350, 500),
            MaximumSize = new Vector2(350, 9999),
        };

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"{Strings.Blacklist_Label} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Labels_Options, 1.0f)
            .AddConfigCheckbox(Strings.Labels_Enabled, Settings.Enabled)
            .Draw();

        if (Settings.Enabled)
        {
            if (Service.ClientState.TerritoryType != 0)
            {
                BlacklistDraw.DrawAddRemoveHere(Settings.BlacklistedZoneSettings);
            }
        
            BlacklistDraw.DrawTerritorySearch(Settings.BlacklistedZoneSettings);
        
            BlacklistDraw.DrawBlacklist(Settings.BlacklistedZoneSettings);
        }
    }
}