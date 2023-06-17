using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.System.Modules;

public class Sage : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Sage;
    public override string DefaultWarningText { get; protected set; } = Strings.SageKardion;

    private const byte MinimumLevel = 4;
    private const byte SageClassJob = 40;
    private const int KardiaStatusId = 2604;
    private const int KardiaActionId = 24285;
    
    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (playerData.MissingClassJob(SageClassJob)) return false;
        if (playerData.GetLevel() < MinimumLevel) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        if (playerData.MissingStatus(KardiaStatusId))
        {
            AddActiveWarning(KardiaActionId, playerData);
        }
    }
}