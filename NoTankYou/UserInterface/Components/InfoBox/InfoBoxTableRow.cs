using System;

namespace NoTankYou.UserInterface.Components.InfoBox;

public class InfoBoxTableRow : DrawList<InfoBoxTableRow>
{
    private readonly InfoBoxTable Owner;

    public Action? FirstColumn => DrawActions.Count > 0 ? DrawActions[0] : null;
    public Action? SecondColumn => DrawActions.Count > 1 ? DrawActions[1] : null;

    public InfoBoxTableRow(InfoBoxTable owner)
    {
        this.Owner = owner;
        DrawListOwner = this;
    }

    public InfoBoxTable EndRow()
    {
        Owner.AddRow(this);

        return Owner;
    }
}