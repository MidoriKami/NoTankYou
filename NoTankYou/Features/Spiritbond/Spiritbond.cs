using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Spiritbond;

public class Spiritbond : ConsumableModuleBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Spiritbond",
        FileName = "Spiritbond",
        IconId = 20645,
        Type = ModuleType.OtherFeatures,
    };

    protected override uint IconId => 20645;
    protected override string IconLabel => "Spiritbond Potion";
    protected override uint StatusId => 49; // Well-Fed
}
