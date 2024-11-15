using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class SpiritBond : ConsumableModule<SpiritBondConfiguration> {
    public override ModuleName ModuleName => ModuleName.SpiritBond;
    protected override string DefaultWarningText => Strings.SpiritBondWarning;
    protected override uint IconId =>Service.DataManager.GetExcelSheet<Item>().GetRow(7059)!.Icon;
    protected override string IconLabel => Service.DataManager.GetExcelSheet<Item>().GetRow(7059)!.Name.ToString();
    protected override uint StatusId => 49; // SpiritBond
}

public class SpiritBondConfiguration() : ConsumableConfiguration(ModuleName.SpiritBond);
