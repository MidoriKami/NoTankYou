using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.WarningBanner.ConfigurationCategories;

public class PositioningConfigurationNode : VerticalListNode {
    public PositioningConfigurationNode(WarningBanner module) {
        FitWidth = true;
        ItemSpacing = 3.0f;
        FitContents = true;

        InitialNodes = [
            new CategoryHeaderNode {
                String = "Positioning",
                Alignment = AlignmentType.Bottom,
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Enable Moving",
                IsChecked = module.Config.EnableMoving,
                OnClick = newValue => {
                    module.Config.EnableMoving = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Enable Resizing",
                IsChecked = module.Config.EnableResizing,
                OnClick = newValue => {
                    module.Config.EnableResizing = newValue;
                    module.Config.MarkDirty();
                },
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitWidth | FlexFlags.FitHeight,
                InitialNodes = [
                    new TextNode {
                        FontSize = 14,
                        AlignmentType = AlignmentType.Left,
                        String = "Scale",
                    },
                    new FloatSliderNode {
                        Min = 0.5f,
                        Max = 3.0f,
                        Value = module.Config.Scale,
                        OnValueChanged = newValue => {
                            module.Config.Scale = newValue;
                            module.Config.MarkDirty();
                        },
                    },
                ],
            },
        ];
    }
}
