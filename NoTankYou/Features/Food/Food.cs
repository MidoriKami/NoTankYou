using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Food;

public class Food : ConsumableModuleBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Food",
        FileName = "Food",
        IconId = 62317,
        Type = ModuleType.OtherFeatures,
    };

    protected override uint IconId => 62317;
    protected override string IconLabel => "Food";
    protected override uint StatusId => 48; // Well-Fed
}
