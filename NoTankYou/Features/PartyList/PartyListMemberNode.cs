using NoTankYou.Classes;

namespace NoTankYou.Features.PartyList;

public class PartyListMemberNode {
    public required PartyListBackgroundNode Background { get; set; }
    public required PartyListForegroundNode Foreground { get; set; }

    public WarningInfo? ActiveWarning {
        set {
            Background.ActiveWarning = value;
            Foreground.ActiveWarning = value;
        }
    }

    public void Update() {
        Background.Update();
        Foreground.Update();
    }
}
