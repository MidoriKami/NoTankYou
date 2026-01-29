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

namespace NoTankYou.Features.Monk;

public class Monk : Module<MonkConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Monk",
        FileName = "Monk",
        IconId = 62020,
        Type = ModuleType.ClassFeatures,
    };
    
    private const byte MinimumLevel = 40;
    private const byte MonkClassJob = 20;
	
    private const uint MantraActionId = 36943;
    private const uint MantraMinimumLevel = 54;
	
    private const uint FormlessFistActionId = 4262;
    private const uint FormlessFistStatusEffect = 2513;
    private const uint FormlessFistMinimumLevel = 52;

    private DateTime lastCombatTime = DateTime.UtcNow;

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ObjectIndex is not 0) return false;
        if (character->ClassJob != MonkClassJob) return false;
        if (character->Level < MinimumLevel) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (Services.Condition.IsInCombat) lastCombatTime = DateTime.UtcNow;

        if (DateTime.UtcNow - lastCombatTime <= TimeSpan.FromSeconds(ModuleConfig.WarningDelay)) return;

        // Mantra
        if (character->Level >= MantraMinimumLevel) {
            if (Services.JobGauges.Get<MNKGauge>().Chakra < 5) {
                GenerateWarning(MantraActionId, "Chakra", character);
            }
        }

        // Formless Fist
        if (character->Level >= FormlessFistMinimumLevel) {
            if (ModuleConfig.FormlessFist && character->MissingStatus(FormlessFistStatusEffect)) {
                GenerateWarning(FormlessFistActionId, "Formless Fist", character);
            }
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
        new CheckboxNode {
            Height = 32.0f,
            String = "Formless Fist Warning",
            IsChecked = ModuleConfig.FormlessFist,
            OnClick = newValue => {
                ModuleConfig.FormlessFist = newValue;
                ModuleConfig.MarkDirty();
            },
        },
    ];
}
