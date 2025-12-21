using KamiLib.Extensions;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;

namespace NoTankYou.Modules;

public class Food : ConsumableModule<FoodConfiguration> {
    public override ModuleName ModuleName => ModuleName.Food;
    protected override string DefaultWarningText => "Food Warning";
    protected override uint IconId => Services.DataManager.GetExcelSheet<Item>().GetRow(30482).Icon;
    protected override string IconLabel => ModuleName.Food.GetDescription();
    protected override uint StatusId => 48; // Well Fed
}

public class FoodConfiguration() : ConsumableConfiguration(ModuleName.Food);
