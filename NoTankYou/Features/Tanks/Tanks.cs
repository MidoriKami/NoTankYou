using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.Interop;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Tanks;

public class Tanks : Module<TanksConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Tanks",
        FileName = "Tanks",
        Type = ModuleType.ClassFeatures,
        IconId = 62019,
    };

    private readonly uint[] tankStanceIdArray = Services.DataManager.GetExcelSheet<Status>()
        .Where(status => status is { InflictedByActor: true, CanStatusOff: true, IsPermanent: true, ParamModifier: 500, PartyListPriority: 0})
        .Select(status => status.RowId)
        .ToArray();

    private const byte MinimumLevel = 10;
    
    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (ModuleConfig.DisableInAlliance && Services.DataManager.CurrentDutyType is DutyType.Alliance) return false;
        if (!character->IsTank) return false;
        if (character->Level < MinimumLevel) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (GroupManager.Instance()->MainGroup.MemberCount is 0) {
            if (character->MissingStatus(tankStanceIdArray)) {
                GenerateWarning(GetActionIdForClass(character->ClassJob), "Missing Tank Stance", character);
            }
        }
        else {
            if (ModuleConfig.CheckAllianceTanks && Services.DataManager.CurrentDutyType is DutyType.Alliance) {
                if (!AllianceMembers.Any(MemberHasTankStance)) {
                    GenerateWarning(GetActionIdForClass(character->ClassJob), "Alliance Missing Tank Stance", character);
                }
            }

            if (!PartyMembers.Any(MemberHasTankStance)) {
                GenerateWarning(GetActionIdForClass(character->ClassJob), "Party Missing Tank Stance", character);
            }
        }
    }

    private unsafe bool MemberHasTankStance(Pointer<BattleChara> member)
        => member.Value is not null && member.Value->IsTank && member.Value->HasStatus(tankStanceIdArray);

    private static uint GetActionIdForClass(byte classJob) => classJob switch {
        1 or 19 => 28u,
        3 or 21 => 48u,
        32 => 3629u,
        37 => 16142u,
        _ => throw new ArgumentOutOfRangeException(nameof(classJob), classJob, null),
    };

    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new CheckboxNode {
            Height = 32.0f,
            String = "Disable in Alliance Raids",
            IsChecked = ModuleConfig.DisableInAlliance,
            OnClick = newValue => {
                ModuleConfig.DisableInAlliance = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Check Alliance Tanks",
            IsChecked = ModuleConfig.CheckAllianceTanks,
            OnClick = newValue => {
                ModuleConfig.CheckAllianceTanks = newValue;
                ModuleConfig.MarkDirty();
            },
        },
    ];
}
