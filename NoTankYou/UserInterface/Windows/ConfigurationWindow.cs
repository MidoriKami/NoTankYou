using System;
using System.Numerics;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NoTankYou.UserInterface.Components;

namespace NoTankYou.UserInterface.Windows;

internal class ConfigurationWindow : Window, IDisposable
{
    private readonly SelectionFrame SelectionFrame;
    private readonly ConfigurationFrame ConfigurationFrame; 

    public ConfigurationWindow() : base($"NoTankYou###NoTankYouMainWindow")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400 * (16.0f / 9.0f), 400),
            MaximumSize = new Vector2(9999,9999)
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;

        var selectables = Service.ModuleManager.GetConfigurationSelectables();

        SelectionFrame = new SelectionFrame(selectables, 0.35f);
        ConfigurationFrame = new ConfigurationFrame();
    }

    public void Dispose()
    {
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {
        SelectionFrame.Draw();

        ConfigurationFrame.Draw(SelectionFrame.Selected);
    }

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "DailyDuty", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }
}