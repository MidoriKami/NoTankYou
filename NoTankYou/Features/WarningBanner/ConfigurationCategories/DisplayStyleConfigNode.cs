using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.WarningBanner.ConfigurationCategories;

public class DisplayStyleConfigNode : VerticalListNode {
    public DisplayStyleConfigNode(WarningBanner module) {
        FitWidth = true;
        ItemSpacing = 3.0f;
        FitContents = true;

        InitialNodes = [
            new CategoryHeaderNode {
                String= "Display Mode",
                Alignment = AlignmentType.Bottom,
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Warning Shield",
                IsChecked = module.Config.ShowWarningShield,
                OnClick = newValue => {
                    module.Config.ShowWarningShield = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Warning Text",
                IsChecked = module.Config.ShowWarningText,
                OnClick = newValue => {
                    module.Config.ShowWarningText = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Player Name",
                IsChecked = module.Config.ShowPlayerName,
                OnClick = newValue => {
                    module.Config.ShowPlayerName = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Action Name",
                IsChecked = module.Config.ShowActionName,
                OnClick = newValue => {
                    module.Config.ShowActionName = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Action Icon",
                IsChecked = module.Config.ShowActionIcon,
                OnClick = newValue => {
                    module.Config.ShowActionIcon = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Enable Animations",
                IsChecked = module.Config.EnableAnimation,
                OnClick = newValue => {
                    module.Config.EnableAnimation = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Enable Action Tooltip",
                IsChecked = module.Config.EnableActionTooltip,
                OnClick = newValue => {
                    module.Config.EnableActionTooltip = newValue;
                    module.Config.MarkDirty();
                },
            },
        ];
    }
}
