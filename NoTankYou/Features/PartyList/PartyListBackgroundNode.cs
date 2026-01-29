using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using NoTankYou.Classes;
using NoTankYou.CustomNodes;

namespace NoTankYou.Features.PartyList;

public class PartyListBackgroundNode : UpdatableNode {
    private readonly SimpleNineGridNode glowNode;

    public PartyListBackgroundNode() {
        DisableCollisionNode = true;
        
        glowNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/PartyListTargetBase.tex",
            TextureSize = new Vector2(48.0f, 48.0f),
            TextureCoordinates = new Vector2(160.0f, 0.0f),
            Offsets = new Vector4(20),
            IsVisible = true,
            Color = Vector4.Zero,
            Alpha = 1.0f,
            AddColor = new Vector3(0.70f, 0.4f, 0.4f),
        };
        glowNode.AttachNode(this);
        
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 120)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(60, 0, AtkTimelineJumpBehavior.LoopForever, 1)
            .AddLabel(61, 2, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(120, 0, AtkTimelineJumpBehavior.LoopForever, 2)
            .EndFrameSet()
            .Build());

        glowNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 60)
            .AddFrame(1, scale: new Vector2(1.0f, 0.97f), alpha: 155)
            .AddFrame(30, scale: new Vector2(1.05f, 1.20f), alpha: 255)
            .AddFrame(60, scale: new Vector2(1.0f, 0.97f), alpha: 155)
            .EndFrameSet()
            .BeginFrameSet(61, 120)
            .AddFrame(61, scale: new Vector2(1.0f, 1.0f), alpha: 255)
            .EndFrameSet()
            .Build());
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        glowNode.Size = Size;
        glowNode.Origin = glowNode.Bounds.Center;
    }

    public override void Update() {
        if (ActiveWarning is null) return;
        if (PartyList.PartyListConfig is not { } config) return;

        glowNode.Color = config.GlowColor;
        glowNode.IsVisible = config.ShowGlow;

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
