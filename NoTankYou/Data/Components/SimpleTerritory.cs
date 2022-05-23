using System;
using Lumina.Excel.GeneratedSheets;

namespace NoTankYou.Data.Components
{
    public class SimpleTerritory : IEquatable<SimpleTerritory>
    {
        public uint TerritoryID { get; }
        public string Name { get; }

        public SimpleTerritory(uint id)
        {
            var row = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(id);

            TerritoryID = row?.RowId ?? 0;
            Name = row?.PlaceName.Value?.Name ?? "Invalid Name";
        }

        public override string ToString()
        {
            return "[ " + TerritoryID + ", " + Name + " ]";
        }

        public bool Equals(SimpleTerritory? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TerritoryID == other.TerritoryID && Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SimpleTerritory) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TerritoryID, Name);
        }
    }
}
