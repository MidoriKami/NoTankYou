using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Utilities;
using NoTankYou.Configuration.Components;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.UserInterface.Windows;

namespace NoTankYou.UserInterface.Components;

internal class SelectionFrame : IDrawable
{
    public ISelectable? Selected { get; private set; }

    private IEnumerable<ISelectable> Selectables { get; }

    private float Weight { get; }

    private readonly string PluginVersion;

    public bool HideDisabled { get; set; }

    public SelectionFrame(IEnumerable<ISelectable> selectables, float weight = 0.30f)
    {
        Selectables = new List<ISelectable>(selectables);
        Weight = weight;

        PluginVersion = GetVersionText();
    }

    public void Draw()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();
        var bottomPadding =  100.0f * ImGuiHelpers.GlobalScale;

        if (ImGui.BeginChild("###SelectionFrame", new Vector2(regionAvailable.X * Weight, 0), false))
        {
            var frameBgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            ImGui.PushStyleColor(ImGuiCol.FrameBg, frameBgColor with { W = 0.05f });

            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
            if (ImGui.BeginListBox("", new Vector2(-1, -bottomPadding)))
            {
                ImGui.PopStyleColor();

                var modules = Selectables.OrderBy(item => item.OwnerModuleName.GetTranslatedString()).ToList();

                if (HideDisabled)
                    modules.RemoveAll(module => !module.ParentModule.GenericSettings.Enabled.Value);

                foreach (var item in modules)
                {
                    ImGui.PushID(item.OwnerModuleName.ToString());

                    var headerHoveredColor = ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderHovered];
                    var textSelectedColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerHoveredColor with { W = 0.1f });
                    ImGui.PushStyleColor(ImGuiCol.Header, textSelectedColor with { W = 0.1f });

                    if (ImGui.Selectable("", Selected == item))
                    {
                        Selected = Selected == item ? null : item;
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();

                    ImGui.SameLine(3.0f);

                    item.DrawLabel();

                    ImGui.Spacing();

                    ImGui.PopID();
                }

                ImGui.EndListBox();
            }
            ImGui.PopStyleVar();

            DrawExtras();

            DrawVersionText();
        }

        ImGui.EndChild();

        ImGui.SameLine();
    }

    private string GetVersionText()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

        var versionString = assemblyInformation[1].Replace('=', ' ');

        var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        return $"{versionString} - {commitInfo}";
    }

    private void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(PluginVersion) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(Colors.Grey, PluginVersion);
    }

    public void DrawExtras()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 3.0f));
        
        DrawPartyOverlayButton(ImGui.GetContentRegionAvail().X);

        DrawBannerOverlayButton(ImGui.GetContentRegionAvail().X);

        DrawBlacklistConfigurationButton(ImGui.GetContentRegionAvail().X);

        ImGui.PopStyleVar();
    }

    private void DrawBlacklistConfigurationButton(float buttonWidth)
    {
        if (ImGui.Button(Strings.TabItems.Blacklist.Button, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<BlacklistConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }
    }

    private void DrawBannerOverlayButton(float buttonWidth)
    {
        if (ImGui.Button(Strings.TabItems.BannerOverlay.Button, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<BannerOverlayConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }
    }

    private void DrawPartyOverlayButton(float buttonWidth)
    {
        var partyOverlaySampleModeEnabled = Service.ConfigurationManager.CharacterConfiguration.PartyOverlay.PreviewMode;

        var colorOrange = PartyListOverlayWindow.AnimationStopwatch.ElapsedMilliseconds > 500 && partyOverlaySampleModeEnabled.Value;

        if (colorOrange)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Orange);
        }

        if (ImGui.Button(Strings.TabItems.PartyOverlay.Button, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<PartyOverlayConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        if (colorOrange)
        {
            ImGui.PopStyleColor();
        }
    }
}