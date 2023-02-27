using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Windows;
using NoTankYou.Configuration;
using NoTankYou.UserInterface.Tabs;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

public class ConfigurationWindow : TabbedSelectionWindow, IDisposable
{
    private readonly List<ISelectionWindowTab> tabs = new()
    {
        new ModuleConfigurationTab(),
        new SettingsTab(),
    };
    
    public ConfigurationWindow() : base($"NoTankYou - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}", 50.0f )
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(550, 400),
            MaximumSize = new Vector2(9999,9999),
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;
    }
    
    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e) => WindowName = $"No Tank You - {e.CharacterData.Name}";

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    protected override IEnumerable<ISelectionWindowTab> GetTabs() => tabs;

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "NoTankYou", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }
    
    protected override void DrawWindowExtras()
    {
        base.DrawWindowExtras();
        PluginVersion.Instance.DrawVersionText();
    }
}