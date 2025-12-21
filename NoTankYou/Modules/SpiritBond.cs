using Lumina.Excel.Sheets;
using NoTankYou.Classes;

namespace NoTankYou.Modules;

public class SpiritBond : ConsumableModule<SpiritBondConfiguration> {
    public override ModuleName ModuleName => ModuleName.SpiritBond;
    protected override string DefaultWarningText => "Spiritbond";
    protected override uint IconId =>Services.DataManager.GetExcelSheet<Item>().GetRow(7059)!.Icon;
    protected override string IconLabel => Services.DataManager.GetExcelSheet<Item>().GetRow(7059)!.Name.ToString();
    protected override uint StatusId => 49; // SpiritBond
}

public class SpiritBondConfiguration() : ConsumableConfiguration(ModuleName.SpiritBond);
