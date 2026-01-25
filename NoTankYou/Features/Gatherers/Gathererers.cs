using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.CustomNodes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Gatherers;

public class Gathererers : Module<GatherersConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Gatherers",
        FileName = "Gatherers",
        IconId = 62017,
        Type = ModuleType.OtherFeatures,
    };
    
    private const byte MinerClassJobId = 16;
    private const byte BotanistClassJobId = 17;
    private const byte FisherClassJobId = 18;

    private record GathererJobData(uint ClassJob, uint MinLevel, uint StatusId, uint ActionId);
    
    private readonly List<GathererJobData> data = [
        new (MinerClassJobId, 1, 225, 227),
        new (MinerClassJobId, 46, 222, 238),
        new (BotanistClassJobId, 1, 217, 210),
        new (BotanistClassJobId, 46, 221, 221),
        new (FisherClassJobId, 61, 1166, 7903),
        new (FisherClassJobId, 65, 1173, 7911),
        new (FisherClassJobId, 50, 805, 4101),
    ];

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character)
        => character->ClassJob is 16 or 17 or 18;

    protected override unsafe void EvaluateWarnings(BattleChara* character)  {
        foreach (var jobData in data) {
            if (!ModuleConfig.Miner && jobData.ClassJob is MinerClassJobId) continue;
            if (!ModuleConfig.Botanist && jobData.ClassJob is BotanistClassJobId) continue;
            if (!ModuleConfig.Fisher && jobData.ClassJob is FisherClassJobId) continue;

            // Collectors Glove
            if (jobData.StatusId is 805 && (!ModuleConfig.CollectorsGlove || character->ClassJob is not FisherClassJobId)) continue;

            if (character->Level >= jobData.MinLevel && character->MissingStatus(jobData.StatusId)) {
                GenerateWarning(jobData.ActionId, "Status Missing", character);
            }
        }
    }

    protected override ICollection<NodeBase> ConfigNodes => [
        new CategoryHeaderNode {
            String = "General Settings",
            Alignment = AlignmentType.Bottom,
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

    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new CheckboxNode {
            Height = 32.0f,
            String = "Miner",
            IsChecked = ModuleConfig.Miner,
            OnClick = newValue => {
                ModuleConfig.Miner = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Botanist",
            IsChecked = ModuleConfig.Botanist,
            OnClick = newValue => {
                ModuleConfig.Botanist = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Fisher",
            IsChecked = ModuleConfig.Fisher,
            OnClick = newValue => {
                ModuleConfig.Fisher = newValue;
                ModuleConfig.MarkDirty();
            },
        },
        new CheckboxNode {
            Height = 32.0f,
            String = "Collectors Glove",
            IsChecked = ModuleConfig.CollectorsGlove,
            OnClick = newValue => {
                ModuleConfig.CollectorsGlove = newValue;
                ModuleConfig.MarkDirty();
            },
        },
    ];
}
