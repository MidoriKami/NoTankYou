using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using NoTankYou.CustomNodes;

namespace NoTankYou.Classes;

public abstract class ConsumableModuleBase : Module<ConsumableModuleConfig> {
    protected abstract uint IconId { get; }
    protected abstract string IconLabel { get; }
    protected abstract uint StatusId { get; }

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (ModuleConfig.SuppressInCombat && Services.Condition.IsInCombat) return false;

        if (ModuleConfig.SavageFilter || ModuleConfig.UltimateFilter || ModuleConfig.ExtremeUnrealFilter || ModuleConfig.CriterionFilter || ModuleConfig.ChaoticFilter) {
            var allowedZones = new List<DutyType>();
            
            if(ModuleConfig.SavageFilter) allowedZones.Add(DutyType.Savage);
            if(ModuleConfig.UltimateFilter) allowedZones.Add(DutyType.Ultimate);
            if(ModuleConfig.ExtremeUnrealFilter) allowedZones.Add(DutyType.Extreme);
            if(ModuleConfig.ExtremeUnrealFilter) allowedZones.Add(DutyType.Unreal);
            if(ModuleConfig.CriterionFilter) allowedZones.Add(DutyType.Criterion);
            if(ModuleConfig.ChaoticFilter) allowedZones.Add(DutyType.ChaoticAlliance);

            var currentCfc = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
            if (currentCfc.RowId is 0) return false;

            var currentDutyType = Services.DataManager.GetDutyType(currentCfc);
            if (!allowedZones.Contains(currentDutyType)) return false;
        }
        
        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        var wellFedStatusIndex = character->StatusManager.GetStatusIndex(StatusId);
        if (wellFedStatusIndex is -1) {
            GenerateWarning(IconId, IconLabel, $"Missing {IconLabel}", character);
            return;
        }

        var statusTimeRemaining = character->StatusManager.GetRemainingTime(wellFedStatusIndex);
        
        if (statusTimeRemaining < ModuleConfig.EarlyWarningTime) {
            if (ModuleConfig is { ShowTimeRemaining: true } && statusTimeRemaining is not 0) {
                GenerateWarning(IconId, IconLabel, $"{IconLabel} Expiring ({statusTimeRemaining:N0}s)", character);
            }
            else {
                GenerateWarning(IconId, IconLabel, $"{IconLabel} Expiring", character);
            }
        }
    }

    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new CheckboxNode {
            Height = 32.0f,
            String = "Suppress in Combat",
            IsChecked = ModuleConfig.SuppressInCombat,
            OnClick = newValue => {
                ModuleConfig.SuppressInCombat = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Show time Remaining",
            IsChecked = ModuleConfig.ShowTimeRemaining,
            OnClick = newValue => {
                ModuleConfig.ShowTimeRemaining = newValue;
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
                    String = "Early Warning Time (s)",
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
        new CategoryHeaderNode {
            String = "Duty Type Filters",
            Alignment = AlignmentType.Bottom,
            TextTooltip = "Enabling any of these will cause warnings to only appear " +
                          "while in the selected content types.",
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Savage",
            IsChecked = ModuleConfig.SavageFilter,
            OnClick = newValue => {
                ModuleConfig.SavageFilter = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Ultimate",
            IsChecked = ModuleConfig.UltimateFilter,
            OnClick = newValue => {
                ModuleConfig.UltimateFilter = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Extreme/Unreal",
            IsChecked = ModuleConfig.ExtremeUnrealFilter,
            OnClick = newValue => {
                ModuleConfig.ExtremeUnrealFilter = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Criterion",
            IsChecked = ModuleConfig.CriterionFilter,
            OnClick = newValue => {
                ModuleConfig.CriterionFilter = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Chaotic",
            IsChecked = ModuleConfig.ChaoticFilter,
            OnClick = newValue => {
                ModuleConfig.ChaoticFilter = newValue;
                ModuleConfig.MarkDirty();
            },
        },
    ];
}
