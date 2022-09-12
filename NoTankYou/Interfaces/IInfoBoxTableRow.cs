using System;

namespace NoTankYou.Interfaces;

internal interface IInfoBoxTableRow
{
    Tuple<Action?, Action?> GetInfoBoxTableRow();
}