using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Windows;
using NoTankYou.Configuration;
using NoTankYou.UserInterface.OverlayWindows;
using NoTankYou.Utilities;

namespace NoTankYou.UserInterface.Windows;

public class ModuleConfigurationWindow : SelectionWindow, IDisposable
{
    public ModuleConfigurationWindow() : base($"NoTankYou - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}", 47.0f )
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

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"No Tank You - {e.CharacterData.Name}";
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    protected override IEnumerable<ISelectable> GetSelectables() => Service.ModuleManager.GetConfigurationSelectables();

    public override void OnClose()
    {
        Service.PluginInterface.UiBuilder.AddNotification("Settings Saved", "NoTankYou", NotificationType.Success);
        Service.ConfigurationManager.Save();
    }

    protected override void DrawExtras()
    {
        DrawNavigationButtons();
        PluginVersion.Instance.DrawVersionText();
    }
    
    private static void DrawNavigationButtons()
    {
        var itemSize = ImGuiHelpers.ScaledVector2(23.0f);
        var region = ImGui.GetContentRegionAvail();
        var padding = ImGui.GetStyle().ItemSpacing;

        var buttonsTotalSize = itemSize * 2 + padding * 1;
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 2.0f - buttonsTotalSize.X / 2.0f, Y = padding.Y});

        var bannerOverlaySampleModeEnabled = Service.ConfigurationManager.CharacterConfiguration.BannerOverlay.SampleMode;
        var partyOverlaySampleModeEnabled = Service.ConfigurationManager.CharacterConfiguration.PartyOverlay.PreviewMode;
        var colorOrange = PartyListOverlayWindow.AnimationStopwatch.ElapsedMilliseconds > 500 && (partyOverlaySampleModeEnabled || bannerOverlaySampleModeEnabled);
        
        if (colorOrange) ImGui.PushStyleColor(ImGuiCol.Text, Colors.Orange);
        if (ImGuiComponents.IconButton("ConfigurationButton", FontAwesomeIcon.Cog)) KamiCommon.WindowManager.ToggleWindowOfType<SettingsConfigurationWindow>();
        if (colorOrange) ImGui.PopStyleColor();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Open Additional Settings Config Window");
        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);

        if (ImGuiComponents.IconButton("KoFiButton", FontAwesomeIcon.Coffee)) Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/midorikami", UseShellExecute = true });
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Support Me on Ko-Fi");
        
        ImGui.PopStyleColor(3);
    }
}