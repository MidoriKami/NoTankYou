using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Color;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.PartyList.ConfigurationCategories;

public class DisplayStyleConfigNode : VerticalListNode {
    public DisplayStyleConfigNode(PartyList module) {
        FitWidth = true;
        ItemSpacing = 3.0f;
        FitContents = true;
        
        InitialNodes = [
            new CategoryHeaderNode {
                String = "Display Style",
                Alignment = AlignmentType.Bottom,
            },
            new ColorEditNode {
                Height = 32.0f,
                String = "Background Color",
                DefaultColor = new Vector4(0.90f, 0.5f, 0.5f, 1.0f),
                CurrentColor = module.Config.GlowColor,
                OnColorConfirmed = newColor => {
                    module.Config.GlowColor = newColor;
                    module.Config.MarkDirty();
                },
                OnColorPreviewed = previewColor => {
                    module.Config.GlowColor = previewColor;
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Background",
                IsChecked = module.Config.ShowGlow,
                OnClick = newValue => {
                    module.Config.ShowGlow = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Icon",
                IsChecked = module.Config.ShowIcon,
                OnClick = newValue => {
                    module.Config.ShowIcon = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Enable Animations",
                IsChecked = module.Config.Animation,
                OnClick = newValue => {
                    module.Config.Animation = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Use Module Icon",
                IsChecked = module.Config.UseModuleIcons,
                OnClick = newValue => {
                    module.Config.UseModuleIcons = newValue;
                    module.Config.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 32.0f,
                String = "Show Tooltips",
                IsChecked = module.Config.Tooltips,
                OnClick = newValue => {
                    module.Config.Tooltips = newValue;
                    module.Config.MarkDirty();
                },
            },
        ];
    }
}
