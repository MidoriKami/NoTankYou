using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.CustomNodes;
using NoTankYou.Enums;

namespace NoTankYou.Features.FreeCompany;

public class FreeCompany : Module<FreeCompanyConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Free Company",
        FileName = "FreeCompany",
        IconId = 60460,
        Type = ModuleType.OtherFeatures,
    };

    private const uint FreeCompanyActionId = 43;
    private readonly uint freeCompanyIconId = (uint) Services.DataManager.GetExcelSheet<CompanyAction>().GetRow(FreeCompanyActionId).Icon;

    private readonly uint[] statusList = Services.DataManager.GetExcelSheet<Status>()
        .Where(status => status.IsFcBuff)
        .Select(status => status.RowId)
        .ToArray();

    protected override void MigrateConfig() {
        ModuleConfig.DisableInSanctuary = false;
        ModuleConfig.WaitForDutyStart = false;
        ModuleConfig.DutiesOnly = false;
        ModuleConfig.SoloMode = false;
    }

    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (Services.Condition.IsBoundByDuty) return false;
        if (character->ObjectIndex is not 0) return false;
        if (character->HomeWorld != character->CurrentWorld) return false;

        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        switch (ModuleConfig.Mode) {
            case FreeCompanyMode.Any when character->MissingStatus(statusList):
            case FreeCompanyMode.Specific when ModuleConfig.BuffCount is 1 && ModuleConfig.PrimaryBuff is not 0 ? character->MissingStatus(ModuleConfig.PrimaryBuff) : character->MissingStatus(ModuleConfig.SecondaryBuff):
            case FreeCompanyMode.Specific when ModuleConfig.BuffCount is 2 && (character->MissingStatus(ModuleConfig.PrimaryBuff) || character->MissingStatus(ModuleConfig.SecondaryBuff)):
                GenerateWarning(freeCompanyIconId, string.Empty, "Free Company Action", character);
                break;
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

    private CheckboxNode? firstBuffCheckbox;
    private LuminaDropDownNode<Status>? firstBuffDropdown;
    private CheckboxNode? secondBuffCheckbox;
    private LuminaDropDownNode<Status>? secondBuffDropdown;
    
    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new HorizontalFlexNode {
            Height = 24.0f,
            AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
            InitialNodes =  [
                new TextNode {
                    AlignmentType = AlignmentType.Left,
                    String = "Warning Mode",
                    TextTooltip = "When in 'Any' mode, warnings will be shown until you have any Free Company buff\n" +
                                  "When in 'Specific' mode, warnings will be shown until have all the buffs selected below",
                },
                new EnumDropDownNode<FreeCompanyMode> {
                    Options = Enum.GetValues<FreeCompanyMode>().ToList(),
                    SelectedOption = ModuleConfig.Mode,
                    OnOptionSelected = newOption => {
                        ModuleConfig.Mode = newOption;
                        ModuleConfig.MarkDirty();
                    },
                },
            ],
        },
        new CategoryHeaderNode {
            String = "Status Selection",
            Alignment = AlignmentType.Bottom,
        },
        new ResNode { Height = 4.0f },
        new HorizontalListNode {
            Height = 24.0f,
            FitHeight = true,
            InitialNodes =  [
                firstBuffCheckbox = new CheckboxNode {
                    Width = 24.0f,
                    IsChecked = ModuleConfig.PrimaryBuff is not 0,
                    OnClick = newValue => {
                        if (!newValue) {
                            ModuleConfig.PrimaryBuff = 0;
                            ModuleConfig.MarkDirty();
                            firstBuffDropdown?.SelectedOption = Services.DataManager.GetExcelSheet<Status>().GetRow(0);
                        }
                    },
                },
                firstBuffDropdown = new LuminaDropDownNode<Status> {
                    Width = 355.0f,
                    FilterFunction = status => status.IsFcBuff,
                    LabelFunction = status => status.Name.ToString(),
                    MaxListOptions = 10,
                    SelectedOption = Services.DataManager.GetExcelSheet<Status>().GetRow(ModuleConfig.PrimaryBuff),
                    OnOptionSelected = newOption => {
                        ModuleConfig.PrimaryBuff = newOption.RowId;
                        ModuleConfig.MarkDirty();
                        firstBuffCheckbox?.IsChecked = ModuleConfig.PrimaryBuff is not 0;
                    },
                },
            ],
        },
        new ResNode { Height = 4.0f },
        new HorizontalListNode {
            Height = 24.0f,
            FitHeight = true,
            InitialNodes =  [
                secondBuffCheckbox = new CheckboxNode {
                    Width = 24.0f,
                    IsChecked = ModuleConfig.SecondaryBuff is not 0,
                    OnClick = newValue => {
                        if (!newValue) {
                            ModuleConfig.SecondaryBuff = 0;
                            ModuleConfig.MarkDirty();
                            secondBuffDropdown?.SelectedOption = Services.DataManager.GetExcelSheet<Status>().GetRow(0);
                        }
                    },
                },
                secondBuffDropdown = new LuminaDropDownNode<Status> {
                    Width = 355.0f,
                    FilterFunction = status => status.IsFcBuff,
                    LabelFunction = status => status.Name.ToString(),
                    MaxListOptions = 10,
                    SelectedOption = Services.DataManager.GetExcelSheet<Status>().GetRow(ModuleConfig.SecondaryBuff),
                    OnOptionSelected = newOption => {
                        ModuleConfig.SecondaryBuff = newOption.RowId;
                        ModuleConfig.MarkDirty();
                        secondBuffCheckbox?.IsChecked = ModuleConfig.SecondaryBuff is not 0;
                    },
                },
            ],
        },
    ];
}
