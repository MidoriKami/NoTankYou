using System;
using System.Linq;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.Interop;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;
using DutyType = KamiLib.Extensions.DutyType;

namespace NoTankYou.Modules;

public unsafe class Tanks : ModuleBase<TankConfiguration> {
    public override ModuleName ModuleName => ModuleName.Tanks;
    protected override string DefaultWarningText => "Tank Stance";

    private readonly uint[] tankClassJobArray = Services.DataManager.GetExcelSheet<ClassJob>()
        .Where(job => job.Role is 1)
        .Select(r => r.RowId)
        .ToArray();

    private readonly uint[] tankStanceIdArray = Services.DataManager.GetExcelSheet<Status>()
        .Where(status => status is { InflictedByActor: true, CanStatusOff: true, IsPermanent: true, ParamModifier: 500, PartyListPriority: 0})
        .Select(status => status.RowId)
        .ToArray();

    private const byte MinimumLevel = 10;
    
    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (Config.DisableInAlliance && IsInAllianceRaid()) return false;
        if (!IsTank(playerData)) return false;
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        if (GroupManager.Instance()->MainGroup.MemberCount is 0) {
            if (playerData.MissingStatus(tankStanceIdArray)) {
                AddActiveWarning(GetActionIdForClass(playerData.GetClassJob()), playerData);
            }
        }
        else {
            if (Config.CheckAllianceTanks && IsInAllianceRaid()) {
                if (AllianceHasStance()) return;
            }

            if (!PartyHasStance()) {
                AddActiveWarning(GetActionIdForClass(playerData.GetClassJob()), playerData);
            }
        }
    }
    
    private bool PartyHasStance() {
        foreach (var partyMember in PartyMemberSpan.PointerEnumerator()) {
            IPlayerData playerData = new PartyMemberPlayerData(partyMember);

            if (!IsTank(playerData)) continue;
            if (playerData.HasStatus(tankStanceIdArray)) return true;
        }

        return false;
    }

    private bool AllianceHasStance() {
        foreach (var partyMember in GroupManager.Instance()->MainGroup.AllianceMembers.PointerEnumerator()) {
            if (partyMember->EntityId is 0xE0000000) continue;
            
            IPlayerData playerData = new PartyMemberPlayerData(partyMember);

            if (!IsTank(playerData)) continue;
            if (playerData.GameObjectHasStatus(tankStanceIdArray)) return true;
        }

        return false;
    }

    private bool IsTank(IPlayerData playerData) {
        if (playerData.MissingClassJob(tankClassJobArray)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;
        
        return true;
    }

    private static bool IsInAllianceRaid()
        => Services.DataManager.GetCurrentDutyType() is DutyType.Alliance;

    private static uint GetActionIdForClass(byte classJob) => classJob switch {
        1 or 19 => 28u,
        3 or 21 => 48u,
        32 => 3629u,
        37 => 16142u,
        _ => throw new ArgumentOutOfRangeException(nameof(classJob), classJob, null),
    };
}

public class TankConfiguration() : ModuleConfigBase(ModuleName.Tanks) {
    public bool DisableInAlliance = true;
    public bool CheckAllianceTanks = true;

    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox("Disable in Alliance Raid", ref DisableInAlliance);

        using var disabled = ImRaii.Disabled(DisableInAlliance);
        ConfigChanged |= ImGui.Checkbox("Check Alliance Tanks", ref CheckAllianceTanks);
        DisabledSettingTooltip(disabled);
    }
}