namespace NoTankYou.UserInterface.Components.InfoBox;

public class InfoBoxList : DrawList<InfoBoxList>
{
    private readonly InfoBox Owner;

    public InfoBoxList(InfoBox owner)
    {
        this.Owner = owner;
        DrawListOwner = this;
    }

    public InfoBox EndList()
    {
        foreach (var row in DrawActions)
        {
            Owner.AddAction(row);
        }

        return Owner;
    }
}