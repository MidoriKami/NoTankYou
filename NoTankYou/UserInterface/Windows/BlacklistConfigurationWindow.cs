using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.BlacklistSystem;
using KamiLib.InfoBoxSystem;
using NoTankYou.Configuration;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Windows;

internal class BlacklistConfigurationWindow : Window, IDisposable
{
    private static BlacklistSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Blacklist;
    
    public BlacklistConfigurationWindow() : base($"{Strings.TabItems.Blacklist.Label} - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(275 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999, 9999)
        };

        Flags |= ImGuiWindowFlags.AlwaysVerticalScrollbar;

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"{Strings.TabItems.Blacklist.Label} - {e.CharacterData.Name}";
    }

    public override void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Common.Labels.Options)
            .AddConfigCheckbox(Strings.Configuration.Enable, Settings.Enabled)
            .Draw();

        if (Settings.Enabled.Value)
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