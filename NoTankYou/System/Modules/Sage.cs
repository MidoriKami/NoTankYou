using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.System.Modules;

public class Sage : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Sage;
    public override string DefaultShortWarning { get; protected set; } = Strings.Sage_WarningTextShort;
    public override string DefaultLongWarning { get; protected set; } = Strings.Sage_WarningText;

    private const byte MinimumLevel = 4;
    private const byte SageClassJob = 40;
    private const int KardiaStatusId = 2604;
    private const int KardiaActionId = 24285;
    
    protected override void EvaluatePlayer(IPlayerData playerData)
    {
        if (playerData.MissingClassJob(SageClassJob)) return;
        if (playerData.GetLevel() < MinimumLevel) return;

        if (playerData.MissingStatus(KardiaStatusId))
        {
            AddActiveWarning(KardiaActionId, playerData);
        }
    }
}