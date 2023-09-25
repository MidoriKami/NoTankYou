using Dalamud.Utility;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;

namespace NoTankYou.System.Modules;

public class SpiritBond : ConsumableModule
{
    public override ModuleName ModuleName => ModuleName.SpiritBond;
    protected override string DefaultWarningText { get; } = Strings.SpiritBondWarning;
    protected override uint IconId { get; set; } = LuminaCache<Item>.Instance.GetRow(7059)!.Icon;
    protected override string IconLabel { get; set; } = LuminaCache<Item>.Instance.GetRow(7059)!.Name.ToDalamudString().ToString();
    protected override uint StatusId { get; set; } = 49; // SpiritBond
}