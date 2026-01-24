using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.PartyList.ConfigurationCategories;

public class FeatureConfigurationNode : VerticalListNode {
    public FeatureConfigurationNode(PartyList module) {
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
            }
        ];
    }
}
