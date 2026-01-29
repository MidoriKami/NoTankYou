using System;
using System.Collections.Generic;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Pictomancer;

public class Pictomancer : Module<PictomancerConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Pictomancer",
        FileName = "Pictomancer",
        IconId = 62042,
        Type = ModuleType.ClassFeatures,
    };
    
    private const byte PictoClassJobId = 42;
    private const uint MinimumLevel = 30;

    private const uint CreatureMinimumLevel = 30;
    private const uint CreatureActionId = 34664;

    private const uint WeaponMinimumLevel = 50;
    private const uint WeaponActionId = 34668;

    private const uint LandscapeMinimumLevel = 70;
    private const uint LandscapeActionId = 34669;

    private DateTime lastCombatTime = DateTime.UtcNow;

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ObjectIndex is not 0) return false;
        if (character->ClassJob is not PictoClassJobId) return false;
        if (character->Level < MinimumLevel) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (Services.Condition.IsInCombat) lastCombatTime = DateTime.UtcNow;

        if (DateTime.UtcNow - lastCombatTime <= TimeSpan.FromSeconds(ModuleConfig.WarningDelay)) return;

        if (character->Level >= CreatureMinimumLevel && !Services.JobGauges.Get<PCTGauge>().CreatureMotifDrawn) {
            GenerateWarning(CreatureActionId, "Creature Motif", character);
        }
        
        if (character->Level >= WeaponMinimumLevel && !Services.JobGauges.Get<PCTGauge>().WeaponMotifDrawn) {
            GenerateWarning(WeaponActionId, "Weapon Motif", character);
        }
        
        if (character->Level >= LandscapeMinimumLevel && !Services.JobGauges.Get<PCTGauge>().LandscapeMotifDrawn) {
            GenerateWarning(LandscapeActionId, "Landscape Motif", character);
        }
    }
    
    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new HorizontalFlexNode {
            Height = 32.0f,
            AlignmentFlags = FlexFlags.FitWidth | FlexFlags.FitHeight,
            InitialNodes = [
                new TextNode {
                    FontSize = 14,
                    AlignmentType = AlignmentType.Left,
                    String = "Warning Delay Time (s)",
                },
                new NumericInputNode {
                    Min = 0,
                    Value = ModuleConfig.WarningDelay,
                    OnValueUpdate = newValue => {
                        ModuleConfig.WarningDelay = newValue;
                        ModuleConfig.MarkDirty();
                    },
                },
            ],
        },
    ];
}
