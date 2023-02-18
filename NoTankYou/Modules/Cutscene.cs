using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using KamiLib.Caching;
using KamiLib.Drawing;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Modules;

public class CutsceneConfiguration : GenericSettings
{
}

public class Cutscene : BaseModule
{
    public override ModuleName Name => ModuleName.Cutscene;
    public override string Command => "cutscene";
    public override List<uint> ClassJobs { get; }
    private static CutsceneConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Cutscene;
    public override GenericSettings GenericSettings => Settings;
    
    private readonly OnlineStatus cutsceneStatus;
    private static readonly Dictionary<uint, Stopwatch> TimeSinceInCutscene = new();

    public Cutscene()
    {
        ClassJobs = LuminaCache<ClassJob>.Instance
            .Select(r => r.RowId)
            .ToList();
        
        cutsceneStatus = LuminaCache<OnlineStatus>.Instance.GetRow(15)!;
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        TimeSinceInCutscene.TryAdd(character.ObjectId, Stopwatch.StartNew());
        var stopwatch = TimeSinceInCutscene[character.ObjectId];

        if (stopwatch.Elapsed >= TimeSpan.FromSeconds(1) && character.HasOnlineStatus(cutsceneStatus.RowId))
        {
            return new WarningState
            {
                MessageLong = Strings.Cutscene_WarningText,
                MessageShort = Strings.Cutscene_WarningText,
                IconID = cutsceneStatus.Icon,
                IconLabel = cutsceneStatus.Name.RawString,
                Priority = Settings.Priority.Value,
            };
        }
        else if (!character.HasOnlineStatus(cutsceneStatus.RowId))
        {
            stopwatch.Restart();
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