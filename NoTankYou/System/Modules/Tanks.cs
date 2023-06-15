using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.System.Modules;

public class TankConfiguration : ModuleConfigBase
{
    [BoolConfigOption("DisableInAllianceRaid", "ModuleOptions", 2)]
    public bool DisableInAllianceRaid = true;

    [BoolConfigOption("CheckAllianceStances", "ModuleOptions", 2)]
    public bool CheckAllianceStances = false;
}

public unsafe class Tanks : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Tanks;
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TankConfiguration();
    public override string DefaultShortWarning { get; protected set; } = Strings.Tank_WarningTextShort;
    public override string DefaultLongWarning { get; protected set; } = Strings.Tank_WarningText;

    private readonly List<uint> tankClassJobs = LuminaCache<ClassJob>.Instance
        .Where(job => job.Role is 1)
        .Select(r => r.RowId)
        .ToList();

    private readonly List<uint> tankStanceIdList = LuminaCache<Status>.Instance
        .Where(status => status is { InflictedByActor: true, CanStatusOff: true, IsPermanent: true, ParamModifier: 500, PartyListPriority: 0})
        .Select(status => status.RowId)
        .ToList();

    private const byte MinimumLevel = 10;
    
    protected override void EvaluatePlayer(IPlayerData playerData)
    {
        if (GetConfig<TankConfiguration>().DisableInAllianceRaid && IsInAllianceRaid()) return;
        if (playerData.MissingClassJob(tankClassJobs.ToArray())) return;
        if (playerData.GetLevel() < MinimumLevel) return;

        if (GroupManager.Instance()->MemberCount is 0)
        {
            if (playerData.MissingStatus(tankStanceIdList.ToArray()))
            {
                AddActiveWarning(GetActionIdForClass(playerData.GetClassJob()), playerData);
            }
        }
        else
        {
            if (GetConfig<TankConfiguration>().CheckAllianceStances)
            {
                if (AllianceHasStance()) return;
            }

            if (!PartyHasStance())
            {
                AddActiveWarning(GetActionIdForClass(playerData.GetClassJob()), playerData);
            }
        }
    }

    private bool AllianceHasStance()
    {
        foreach (var partyMember in GroupManager.Instance()->AllianceMembersSpan)
        {
            if (HasTankStance(new PartyMemberPlayerData(&partyMember))) return true;
        }

        return false;
    }

    private bool PartyHasStance()
    {
        var filteredPartyMembers = new Span<PartyMember>(GroupManager.Instance()->PartyMembers, GroupManager.Instance()->MemberCount);

        foreach (var partyMember in filteredPartyMembers)
        {
            if (HasTankStance(new PartyMemberPlayerData(&partyMember))) return true;
        }

        return false;
    }

    private bool HasTankStance(IPlayerData playerData)
    {
        if (playerData.MissingClassJob(tankClassJobs.ToArray())) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        if (playerData.HasStatus(tankStanceIdList.ToArray())) return true;

        return false;
    }

    private static bool IsInAllianceRaid()
    {
        var currentTerritory = Service.ClientState.TerritoryType;

        return DutyLists.Instance.IsType(currentTerritory, DutyType.Alliance);
    }
    
    private static uint GetActionIdForClass(byte classJob) => classJob switch
    {
        1 or 19 => 28u,
        3 or 21 => 48u,
        32 => 3629u,
        37 => 16142u,
        _ => throw new ArgumentOutOfRangeException(nameof(classJob), classJob, null),
    };
}