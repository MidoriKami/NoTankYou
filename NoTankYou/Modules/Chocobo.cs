using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class ChocoboConfiguration : GenericSettings
{
    public Setting<int> EarlyWarningTime = new(60);
    public Setting<bool> EnableEarlyWarning = new(true);
}

public unsafe class Chocobo : BaseModule
{
    public override ModuleName Name => ModuleName.Chocobo;
    public override string Command => "chocobo";
    public override List<uint> ClassJobs { get; }
    public static ChocoboConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Chocobo;
    public override GenericSettings GenericSettings => Settings;

    private readonly Item gysahlGreens;

    public Chocobo()
    {
        ClassJobs = LuminaCache<ClassJob>.Instance
            .Select(r => r.RowId)
            .ToList();

        gysahlGreens = LuminaCache<Item>.Instance.GetRow(4868)!;
    }
    
    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (Service.DutyState.IsDutyStarted) return null;
        
        var remainingTimeLimit = Settings.EnableEarlyWarning ? Settings.EarlyWarningTime.Value : 0;
        
        if (UIState.Instance()->Buddy.TimeLeft <= remainingTimeLimit)
        {
            return new WarningState
            {
                MessageLong = Strings.Chocobo_WarningText,
                MessageShort = Strings.Chocobo_WarningText,
                IconLabel = gysahlGreens.Name.ToDalamudString().TextValue,
                IconID = gysahlGreens.Icon,
                Priority = Settings.Priority.Value,
            };
        }

        return null;
    }
    
    public override void DrawConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Tabs_Settings)
            .AddConfigCheckbox(Strings.Labels_Enabled, Settings.Enabled)
            .AddInputInt(Strings.Labels_Priority, Settings.Priority, 0, 10)
            .Draw();
            
        InfoBox.Instance.DrawOverlaySettings(Settings);
            
        InfoBox.Instance.DrawOptions(Settings);
    }
}