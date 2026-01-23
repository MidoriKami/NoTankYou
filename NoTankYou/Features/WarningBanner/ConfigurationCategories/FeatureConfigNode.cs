using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;
using NoTankYou.Enums;

namespace NoTankYou.Features.WarningBanner.ConfigurationCategories;

public class FeatureConfigurationNode : VerticalListNode {
    public FeatureConfigurationNode(WarningBanner module) {
        FitWidth = true;
        ItemSpacing = 3.0f;
        FitContents = true;
        
        InitialNodes = [
            new CategoryHeaderNode {
                String = "Feature Configuration",
                Alignment = AlignmentType.Bottom,
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Only Show Warnings for Yourself",
                IsChecked = module.Config.SoloMode,
                OnClick = newValue => {
                    module.Config.SoloMode = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Sample Mode",
                IsChecked = System.WarningController.SampleModeEnabled,
                OnClick = newValue => System.WarningController.ToggleSampleMode(newValue),
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitWidth | FlexFlags.CenterVertically,
                InitialNodes = [
                    new TextNode {
                        Height = 32.0f,
                        FontSize = 14,
                        AlignmentType = AlignmentType.Left,
                        String = "Display Mode",
                    },
                    new EnumDropDownNode<BannerDisplayMode> {
                        Height = 24.0f,
                        Options = Enum.GetValues<BannerDisplayMode>().ToList(),
                        SelectedOption = module.Config.DisplayMode,
                        OnOptionSelected = newValue => {
                            module.Config.DisplayMode = newValue;
                            module.Config.MarkDirty();
                        },
                    },
                ],
            },
        ];
    }
}
