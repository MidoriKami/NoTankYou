using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Windows;
using NoTankYou.UserInterface.Components;

namespace NoTankYou.UserInterface.Windows;

public class SettingsConfigurationWindow : SelectionWindow
{
    private readonly List<ISelectable> selectables = new()
    {
        new BannerOverlayConfigurationSelectable(),
        new PartyOverlayConfigurationSelectable(),
        new BlacklistConfigurationSelectable(),
    };
    
    public SettingsConfigurationWindow() : base("Extra Settings Configuration Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(550, 400),
            MaximumSize = new Vector2(9999,9999),
        };

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    protected override IEnumerable<ISelectable> GetSelectables() => selectables;
}