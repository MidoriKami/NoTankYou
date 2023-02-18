using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Extensions;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace NoTankYou.Modules;

internal record IconInfo(uint ID, string Name);

public class TankConfiguration : GenericSettings
{
    public Setting<bool> DisableInAllianceRaid = new(true);
    public Setting<bool> CheckAllianceStances = new(false);
}

public class Tanks : BaseModule
{
    public override ModuleName Name => ModuleName.Tanks;
    public override string Command => "tank";
    public override sealed List<uint> ClassJobs { get; }
    private static TankConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.Tank;
    public override GenericSettings GenericSettings => Settings;

    private readonly List<uint> tankStances = new() { 79, 743, 91, 1833 };
    private readonly Dictionary<uint, IconInfo> tankIcons = new();
    
    public Tanks()
    {
        ClassJobs = LuminaCache<ClassJob>.Instance
            .Where(job => job.Role is 1)
            .Select(r => r.RowId)
            .ToList();

        foreach (var job in ClassJobs)
        {
            var icon = GetTankIcon(job);

            tankIcons.Add(job, icon);
        }
    }

    public override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (character.Level < 10) return null;

        if (Settings.DisableInAllianceRaid && DutyLists.Instance.IsType(Service.ClientState.TerritoryType, DutyType.Alliance)) return null;

        if (Settings.CheckAllianceStances && DutyLists.Instance.IsType(Service.ClientState.TerritoryType, DutyType.Alliance))
        {
            return EvaluateAllianceStances(character);
        }

        if (Service.PartyList.Length == 0)
        {
            return character.HasStatus(tankStances) ? null : TankWarning(character);
        }
        else
        {
            return EvaluateParty(character);
        }
    }

    private WarningState? EvaluateParty(Character character)
    {
        var tanks = Service.PartyList
            .WithJob(ClassJobs)
            .Alive()
            .ToList();

        if (tanks.Any() && !tanks.WithStatus(tankStances).Any())
        {
            return TankWarning(character);
        }

        return null;
    }

    private WarningState? EvaluateAllianceStances(PlayerCharacter character)
    {
        var partyTanks = Service.PartyList
            .WithJob(ClassJobs)
            .Alive()
            .ToList();

        var allianceTanks = GetAllianceTanks();

        var partyMissingStance = !partyTanks.WithStatus(tankStances).Any();
        var allianceMissingStance = !allianceTanks.WithStatus(tankStances).Any();

        if (partyMissingStance && allianceMissingStance)
        {
            return TankWarning(character);
        }

        return null;
    }
        
    private IconInfo GetTankIcon(uint classjob)
    {
        var actionId = classjob switch
        {
            1 or 19 => 28u,
            3 or 21 => 48u,
            32 => 3629u,
            37 => 16142u,
            _ => throw new ArgumentOutOfRangeException(nameof(classjob), classjob, null),
        };

        var action = LuminaCache<Action>.Instance.GetRow(actionId);

        return new IconInfo(action!.Icon, action.Name.RawString);
    }

    private IEnumerable<PlayerCharacter> GetAllianceTanks()
    {
        var players = new List<PlayerCharacter>();

        for (var i = 0; i < 16; ++i)
        {
            var player = HudHelper.GetAllianceMember(i);
            if(player == null) continue;

            if (ClassJobs.Contains(player.ClassJob.Id))
            {
                players.Add(player);
            }
        }

        return players;
    }

    private WarningState TankWarning(Character character)
    {
        var iconInfo = tankIcons[character.ClassJob.Id];

        return new WarningState
        {
            MessageShort = Strings.Tank_WarningTextShort,
            MessageLong = Strings.Tank_WarningText,
            IconID = iconInfo.ID,
            IconLabel = iconInfo.Name,
            Priority = Settings.Priority.Value,
        };
    }

    protected override void DrawExtraConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Common_AdditionalOptions)
            .AddConfigCheckbox(Strings.Tank_DisableInAllianceRaid, Settings.DisableInAllianceRaid)
            .AddConfigCheckbox(Strings.Tank_CheckAllianceStances, Settings.CheckAllianceStances)
            .Draw();
    }
}