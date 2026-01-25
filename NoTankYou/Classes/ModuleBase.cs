using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;
using Action = Lumina.Excel.Sheets.Action;

namespace NoTankYou.Classes;

public abstract unsafe class ModuleBase : FeatureBase {
    
    public abstract ConfigBase ConfigBase { get; }

    public readonly List<WarningInfo> ActiveWarnings = [];

    public bool HasWarnings => ActiveWarnings.Count is not 0;

    protected List<Pointer<BattleChara>> BattleCharacters = [];
    protected List<Pointer<BattleChara>> PartyMembers = [];
    protected List<Pointer<BattleChara>> AllianceMembers = [];
    
    protected sealed override void OnFeatureEnable() { }
    protected sealed override void OnFeatureDisable() { }
        
    protected abstract bool ShouldEvaluateWarnings(BattleChara* character);
    protected abstract void EvaluateWarnings(BattleChara* character);

    private readonly HashSet<ulong> suppressedObjectIds = [];
    private readonly Dictionary<ulong, Stopwatch> suppressionTimer = new();
    private readonly DeathTracker deathTracker = new();

    protected sealed override void OnFeatureUpdate() {
        if (ConfigBase.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ConfigBase.Save();
        }

        ActiveWarnings.Clear();
        BattleCharacters.Clear();
        PartyMembers.Clear();

        if (!IsEnabled) return;
        if (Services.ClientState.IsPvPExcludingDen) return;
        if (Services.Condition.IsCrossWorld) return;
        if (Services.Condition.IsInCutsceneOrQuestEvent) return;
        if (ConfigBase.BlacklistedZones.Contains(Services.ClientState.TerritoryType)) return;
        if (ConfigBase.DisableInSanctuary && TerritoryInfo.Instance()->InSanctuary) return;
        if (ConfigBase.WaitForDutyStart && Services.Condition.IsBoundByDuty && !Services.DutyState.IsDutyStarted) return;
        if (ConfigBase.DutiesOnly && !Services.Condition.IsBoundByDuty) return;

        // Collect the battle character pointers for use in each module's logic.
        foreach (var characterEntry in CharacterManager.Instance()->BattleCharas) {
            if (characterEntry.Value is null) continue;
            if (characterEntry.Value->ObjectKind is not ObjectKind.Pc) continue;

            BattleCharacters.Add(characterEntry);

            if (characterEntry.Value->IsPartyMember) {
                PartyMembers.Add(characterEntry);
            }

            if (characterEntry.Value->IsAllianceMember) {
                AllianceMembers.Add(characterEntry);
            }
        }

        foreach (var characterEntry in BattleCharacters) {
            var battleCharacter = characterEntry.Value;
            if (battleCharacter is null) continue;
            
            if (ConfigBase.SoloMode && battleCharacter->ObjectIndex is not 0) continue;
            if (ConfigBase.PartyMembersOnly && !battleCharacter->IsPartyMember && battleCharacter->ObjectIndex is not 0) continue;

            ProcessPlayer(battleCharacter);
        }
    }

    private void ProcessPlayer(BattleChara* character) {
        if (character->EntityId is 0xE0000000 or 0) return;
        if (HasDisallowedCondition()) return;
        if (character->HasStatus(1534)) return; // Is Role-Playing Status
        if (deathTracker.IsDead(character)) return;
        if (!ShouldEvaluateWarnings(character)) return;
        if (suppressedObjectIds.Contains(character->EntityId)) return;

        EvaluateWarnings(character);
        EvaluateAutoSuppression(character);
    }
    
    protected void GenerateWarning(uint actionId, string warningText, BattleChara* battleChara) => ActiveWarnings.Add(new WarningInfo {
        Priority = ConfigBase.Priority,
        IconId = Services.DataManager.GetExcelSheet<Action>().GetRow(actionId).Icon,
        ActionId = actionId,
        IconLabel = Services.DataManager.GetExcelSheet<Action>().GetRow(actionId).Name.ToString(),
        Message = ConfigBase.CustomWarningText.IsNullOrEmpty() ? warningText : ConfigBase.CustomWarningText,
        SourceCharacter = battleChara,
        SourceModule = ModuleInfo.DisplayName,
        ModuleIcon = ModuleInfo.IconId,
    });

    protected void GenerateWarning(uint iconId, string iconLabel, string warningText, BattleChara* battleChara) => ActiveWarnings.Add(new WarningInfo {
        Priority = ConfigBase.Priority,
        IconId = iconId,
        ActionId = 0,
        IconLabel = iconLabel,
        Message = ConfigBase.CustomWarningText.IsNullOrEmpty() ? warningText : ConfigBase.CustomWarningText,
        SourceCharacter = battleChara,
        SourceModule = ModuleInfo.DisplayName,
        ModuleIcon = ModuleInfo.IconId,
    });
    
    private void EvaluateAutoSuppression(BattleChara* character) {
        if (!ConfigBase.AutoSuppress) return;

        // Do not allow auto suppression for the user.
        if (character->ObjectIndex is 0) return;

        suppressionTimer.TryAdd(character->EntityId, Stopwatch.StartNew());
        if (!suppressionTimer.TryGetValue(character->EntityId, out var timer)) return;

        if (HasWarnings) {
            if (timer.Elapsed.TotalSeconds >= ConfigBase.AutoSuppressTime) {
                suppressedObjectIds.Add(character->EntityId);
                Services.PluginLog.Warning($"[{Name}]: Adding {character->GetName()} to auto-suppression list");
            }
        }
        else {
            timer.Restart();
        }
    }

    private static bool HasDisallowedCondition()
        => Services.Condition.Any(ConditionFlag.Jumping61, ConditionFlag.Transformed, ConditionFlag.InThisState89);

    public sealed override NodeBase DisplayNode => new ScrollingListNode {
        FitWidth = true,
        ItemSpacing = 2.0f,
        InitialNodes = [
            ..ConfigNodes,
            ..GetModuleSpecificEntries(),
            // Maybe Blacklist Features here?
        ],
    };

    protected virtual ICollection<NodeBase> ConfigNodes => [
        new CategoryHeaderNode {
            String = "General Settings",
            Alignment = AlignmentType.Bottom,
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Wait for Duty Start",
            IsChecked = ConfigBase.WaitForDutyStart,
            OnClick = newValue => {
                ConfigBase.WaitForDutyStart = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Show in Duties Only",
            IsChecked = ConfigBase.DutiesOnly,
            OnClick = newValue => {
                ConfigBase.DutiesOnly = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Disable in Sanctuaries",
            IsChecked = ConfigBase.DisableInSanctuary,
            OnClick = newValue => {
                ConfigBase.DisableInSanctuary = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Only Check Self",
            IsChecked = ConfigBase.SoloMode,
            OnClick = newValue => {
                ConfigBase.SoloMode = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Exclude Non-Party Members",
            IsChecked = ConfigBase.PartyMembersOnly,
            OnClick = newValue => {
                ConfigBase.PartyMembersOnly = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Enable Warning Auto-Suppression",
            TextTooltip = "After the specified amount of time has elapsed, automatically suppresses warnings for players.",
            IsChecked = ConfigBase.AutoSuppress,
            OnClick = newValue => {
                ConfigBase.AutoSuppress = newValue;
                ConfigBase.MarkDirty();
            },
        },
        new HorizontalFlexNode {
            Height = 32.0f,
            AlignmentFlags = FlexFlags.FitWidth | FlexFlags.FitHeight,
            InitialNodes = [
                new TextNode {
                    FontSize = 14,
                    AlignmentType = AlignmentType.Left,
                    String = "Auto-Suppression Time (s)",
                },
                new NumericInputNode {
                    Value = ConfigBase.AutoSuppressTime,
                    OnValueUpdate = newValue => {
                        newValue = Math.Clamp(newValue, 5, 600);
                        ConfigBase.AutoSuppressTime = newValue;
                        ConfigBase.MarkDirty();
                    },
                },
            ],
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
                    Value = ConfigBase.Priority,
                    OnValueUpdate = newValue => {
                        ConfigBase.Priority = newValue;
                        ConfigBase.MarkDirty();
                    },
                },
            ],
        },
        new TextInputNode {
            Height = 32.0f,
            PlaceholderString = "Custom Warning Text",
            String = ConfigBase.CustomWarningText,
            OnInputReceived = newString => {
                ConfigBase.CustomWarningText = newString.ToString();
                ConfigBase.MarkDirty();
            },
        },
    ];

    protected virtual ICollection<NodeBase> ModuleConfigNodes => [];
    
    private ICollection<NodeBase> GetModuleSpecificEntries() {
        var configEntries = ModuleConfigNodes;
        if (configEntries.Count is not 0) {
            return [
                new CategoryHeaderNode {
                    String = "Module Settings",
                    Alignment = AlignmentType.Bottom,
                },
                ..configEntries,
            ];
        }

        return [];
    }
}
