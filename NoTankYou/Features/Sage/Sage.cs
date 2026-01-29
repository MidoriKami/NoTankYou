using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using KamiToolKit;
using KamiToolKit.Nodes;
using NoTankYou.Classes;
using NoTankYou.Enums;

namespace NoTankYou.Features.Sage;

public class Sage : Module<SageConfig> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Sage",
        FileName = "Sage",
        IconId = 62040,
        Type = ModuleType.ClassFeatures,
    };
    
    private const byte MinimumLevel = 4;
    private const byte SageClassJob = 40;
    private const int KardiaStatusId = 2604;
    private const int KardiaActionId = 24285;


    protected override unsafe bool ShouldEvaluateWarnings(BattleChara* character) {
        if (ModuleConfig.DisableWhileSolo && GroupManager.Instance()->MainGroup.MemberCount is 0) return false;
        if (character->Level < MinimumLevel) return false;
        if (character->ClassJob is not SageClassJob) return false;
        
        return true;
    }

    protected override unsafe void EvaluateWarnings(BattleChara* character) {
        if (character->MissingStatus(KardiaStatusId)) {
            GenerateWarning(KardiaActionId, "Sage Kardion", character);
        }
    }

    protected override ICollection<NodeBase> ModuleConfigNodes => [
        new CheckboxNode {
            Height = 32.0f,
            String = "Disable While Solo",
            IsChecked = ModuleConfig.DisableWhileSolo,
            OnClick = newValue => {
                ModuleConfig.DisableWhileSolo = newValue;
                ModuleConfig.MarkDirty();
            },
        },
    ];
}
