using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.CustomNodes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Chocobo;

public class Chocobo : Module<ChocoboConfig> {

    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Chocobo",
        FileName = "Chocobo",
        IconId = 62043,
        Type = ModuleType.OtherFeatures,
    };
    
    private static Item GyshalGreensItem => Services.DataManager.GetExcelSheet<Item>().GetRow(GyshalGreensItemId);

    private const uint GyshalGreensItemId = 4868;
    private readonly uint gyshalGreensIconId = GyshalGreensItem.Icon;
    private readonly string gyshalGreensActionName = GyshalGreensItem.Name.ToString();

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (character->ObjectIndex is not 0) return false;
        if (!Services.Condition.CanSummonChocobo) return false;
        if (Services.Condition.IsBoundByDuty) return false;
        if (ModuleConfig.DisableInCombat && Services.Condition.IsInCombat) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        var timeRemaining = UIState.Instance()->Buddy.CompanionInfo.TimeLeft;

        if (timeRemaining is 0) {
            GenerateWarning(gyshalGreensIconId, gyshalGreensActionName, "Chocobo Missing", character);
        }
        else if (ModuleConfig.EarlyWarningTime is not 0 && timeRemaining <= ModuleConfig.EarlyWarningTime) {
            GenerateWarning(gyshalGreensIconId, gyshalGreensActionName, "Chocobo Expiring", character);
        }
    }

    protected override ICollection<NodeBase> ConfigNodes => [
        new CategoryHeaderNode {
            String = "General Settings",
            Alignment = AlignmentType.Bottom,
        },
        new HorizontalFlexNode {
            Height = 32.0f,
            AlignmentFlags = FlexFlags.FitWidth | FlexFlags.FitHeight,
            InitialNodes = [
                new TextNode {
                    FontSize = 14,
                    AlignmentType = AlignmentType.Left,
                    String = "Priority",
                },
                new NumericInputNode {
                    Min = -64,
                    Max = 64,
                    Value = ModuleConfig.Priority,
                    OnValueUpdate = newValue => {
                        ModuleConfig.Priority = newValue;
                        ModuleConfig.MarkDirty();
                    },
                },
            ],
        },
        new TextInputNode {
            Height = 32.0f,
            PlaceholderString = "Custom Warning Text",
            String = ModuleConfig.CustomWarningText,
            OnInputReceived = newString => {
                ModuleConfig.CustomWarningText = newString.ToString();
                ModuleConfig.MarkDirty();
            },
        },
    ];

    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new CheckboxNode {
            Height = 32.0f,
            String = "Disable in Combat",
            IsChecked = ModuleConfig.DisableInCombat,
            OnClick = newValue => {
                ModuleConfig.DisableInCombat = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new HorizontalFlexNode {
            Height = 32.0f,
            AlignmentFlags = FlexFlags.FitWidth | FlexFlags.FitHeight,
            InitialNodes = [
                new TextNode {
                    FontSize = 14,
                    AlignmentType = AlignmentType.Left,
                    String = "Early Warning Time",
                    TextTooltip = "Set to 0 to disable early warning feature.",
                },
                new NumericInputNode {
                    Min = 0,
                    Value = ModuleConfig.EarlyWarningTime,
                    OnValueUpdate = newValue => {
                        ModuleConfig.EarlyWarningTime = newValue;
                        ModuleConfig.MarkDirty();
                    },
                },
            ],
        },
    ];
}
