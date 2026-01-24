using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using NoTankYou.Classes;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.PartyList;

public unsafe class PartyListForegroundNode : UpdatableNode {
    private readonly IconImageNode warningIconNode;

    public PartyListForegroundNode() {
        DisableCollisionNode = true;

        warningIconNode = new IconImageNode {
            Size = new Vector2(16.0f, 16.0f),
            Origin = new Vector2(8.0f, 8.0f),
            Position = new Vector2(24.0f, 18.0f) + new Vector2(16.0f, 0.0f),
            TextureSize = new Vector2(28.0f, 28.0f),
            IsVisible = true,
            FitTexture = true,
        };
        warningIconNode.AttachNode(this);
        
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 120)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
            .AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
            .EndFrameSet()
            .Build());

        warningIconNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 60)
            .AddFrame(1, scale: new Vector2(1.0f, 1.0f), alpha: 155)
            .AddFrame(30, scale: new Vector2(1.55f, 1.55f), alpha: 255)
            .AddFrame(60, scale: new Vector2(1.0f, 1.0f), alpha: 155)
            .EndFrameSet()
            .BeginFrameSet(61, 120)
            .AddFrame(61, scale: new Vector2(1.55f, 1.55f), alpha: 255)
            .EndFrameSet()
            .Build());
    }

    private bool? tooltipsEnabled;
    
    public override void Update() {
        if (ActiveWarning is null) return;
        if (PartyList.PartyListConfig is not { } config) return;

        if (config.UseModuleIcons) {
            warningIconNode.IconId = ActiveWarning.ModuleIcon;
        }
        else {
            warningIconNode.IconId = 60074;
        }
        
        if (config.Tooltips) {
            warningIconNode.TextTooltip = ActiveWarning.Message;
            warningIconNode.ActionTooltip = ActiveWarning.ActionId;
            warningIconNode.AddNodeFlags(NodeFlags.HasCollision);
        }
        else {
            warningIconNode.TextTooltip = string.Empty;
            warningIconNode.ActionTooltip = 0;
            warningIconNode.RemoveNodeFlags(NodeFlags.HasCollision);
        }
        
        if (tooltipsEnabled is null || tooltipsEnabled != config.Tooltips) {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(this);
            if (addon is not null) {
                addon->UpdateCollisionNodeList(false);
            }

            tooltipsEnabled = config.Tooltips;
        }
        
        warningIconNode.IsVisible = config.ShowIcon;
        
        Timeline?.PlayAnimation(config.Animation ? 1 : 2);
    }

    public WarningInfo? ActiveWarning {
        get;
        set {
            field = value;
            IsVisible = value is not null;
        }
    }
}
