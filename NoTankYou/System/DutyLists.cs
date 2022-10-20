using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Lumina.Excel.GeneratedSheets;

namespace NoTankYou.System;

public enum DutyType
{
    Savage,
    Ultimate,
    ExtremeUnreal,
    Criterion,
    None,
}

internal class DutyLists : IDisposable
{
    public List<uint> Savage { get; init; }
    public List<uint> Ultimate { get; init; }
    public List<uint> ExtremeUnreal { get; init; }
    public List<uint> Criterion { get; init; }
    
    public DutyLists()
    {
        // ContentType.Row 5 == Raids
        Savage = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
            .Where(t => t.ContentType.Row == 5)
            .Where(t => t.Name.RawString.Contains("Savage"))
            .Select(r => r.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 28 == Ultimate Raids
        Ultimate = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
            .Where(t => t.ContentType.Row == 28)
            .Select(t => t.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 4 == Trials
        ExtremeUnreal = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
            .Where(t => t.ContentType.Row == 4)
            .Where(t => t.Name.RawString.Contains("Extreme") || t.Name.RawString.Contains("Unreal"))
            .Select(t => t.TerritoryType.Row)
            .ToList();

        Criterion = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
            .Where(row => row.ContentType.Row is 30)
            .Select(row => row.RowId)
            .ToList();
    }

    public DutyType GetDutyType(uint id)
    {
        if (Savage.Contains(id)) return DutyType.Savage;
        if (Ultimate.Contains(id)) return DutyType.Ultimate;
        if (ExtremeUnreal.Contains(id)) return DutyType.ExtremeUnreal;
        if (Criterion.Contains(id)) return DutyType.Criterion;

        return DutyType.None;
    }
    
    public void Dispose()
    {
        
    }
}