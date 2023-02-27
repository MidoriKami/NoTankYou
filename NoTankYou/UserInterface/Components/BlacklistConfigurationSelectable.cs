using ImGuiNET;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.ZoneFilterList;
using NoTankYou.DataModels;
using NoTankYou.Localization;

namespace NoTankYou.UserInterface.Components;

public class BlacklistConfigurationSelectable : ISelectable, IDrawable
{
    public IDrawable Contents => this;

    public string ID => Strings.Blacklist_Label;
    
    private static BlacklistSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Blacklist;
    
    public void DrawLabel() => ImGui.Text(ID);

    public void Draw()
    {
        InfoBox.Instance
            .AddTitle(Strings.Labels_Options, 1.0f)
            .AddConfigCheckbox(Strings.Labels_Enabled, Settings.Enabled)
            .Draw();

        if (Settings.Enabled)
        {
            if (Service.ClientState.TerritoryType != 0)
            {
                ZoneFilterListDraw.DrawAddRemoveHere(Settings.BlacklistedZoneSettings);
            }
        
            ZoneFilterListDraw.DrawTerritorySearch(Settings.BlacklistedZoneSettings, ZoneFilterType.BlackList);
        
            ZoneFilterListDraw.DrawZoneList(Settings.BlacklistedZoneSettings, ZoneFilterType.BlackList);
        }
    }
}