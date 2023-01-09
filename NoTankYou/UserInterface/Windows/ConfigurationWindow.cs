using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using KamiLib;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Windows;
using NoTankYou.Configuration;
using NoTankYou.Localization;
using NoTankYou.UserInterface.OverlayWindows;

namespace NoTankYou.UserInterface.Windows;

public class ConfigurationWindow : SelectionWindow, IDisposable
{
    private static readonly string PluginVersion = GetVersionText();
    
    public ConfigurationWindow() : base($"NoTankYou - {Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}", 0.40f, 100.0f )
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
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 3.0f));

        DrawPartyOverlayButton(ImGui.GetContentRegionAvail().X);

        DrawBannerOverlayButton(ImGui.GetContentRegionAvail().X);

        DrawBlacklistConfigurationButton(ImGui.GetContentRegionAvail().X);

        DrawVersionText();

        ImGui.PopStyleVar();
    }

    private static string GetVersionText()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

        var versionString = assemblyInformation[1].Replace('=', ' ');

        var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        return $"{versionString} - {commitInfo}";
    }

    private static void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(PluginVersion) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(Colors.Grey, PluginVersion);
    }

    private static void DrawBlacklistConfigurationButton(float buttonWidth)
    {
        if (ImGui.Button(Strings.Blacklist_Button, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            if (KamiCommon.WindowManager.GetWindowOfType<BlacklistConfigurationWindow>() is { } window)
            {
                window.IsOpen = !window.IsOpen;
            }
        }
    }

    private static void DrawBannerOverlayButton(float buttonWidth)
    {
        if (ImGui.Button(Strings.BannerOverlay_Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            if (KamiCommon.WindowManager.GetWindowOfType<BannerOverlayConfigurationWindow>() is { } window)
            {
                window.IsOpen = !window.IsOpen;
            }
        }
    }
    
    private static void DrawPartyOverlayButton(float buttonWidth)
    {
        var partyOverlaySampleModeEnabled = Service.ConfigurationManager.CharacterConfiguration.PartyOverlay.PreviewMode;

        var colorOrange = PartyListOverlayWindow.AnimationStopwatch.ElapsedMilliseconds > 500 && partyOverlaySampleModeEnabled.Value;

        if (colorOrange)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Orange);
        }

        if (ImGui.Button(Strings.PartyOverlay_Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            if (KamiCommon.WindowManager.GetWindowOfType<PartyOverlayConfigurationWindow>() is { } window)
            {
                window.IsOpen = !window.IsOpen;
            }
        }

        if (colorOrange)
        {
            ImGui.PopStyleColor();
        }
    }
}