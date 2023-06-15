using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.System.Modules;

public class Scholar : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Scholar;
    public override string DefaultShortWarning { get; protected set; } = Strings.Scholar_WarningTextShort;
    public override string DefaultLongWarning { get; protected set; } = Strings.Scholar_WarningText;

    private const int DissipationStatusId = 791;
    private const byte ScholarJobId = 28;
    private const uint SummonEosActionId = 17215;
    private const int MinimumLevel = 4;

    private readonly Debouncer dissipationDebouncer = new();
    
    protected override void EvaluatePlayer(IPlayerData playerData)
    {
        if (playerData.MissingClassJob(ScholarJobId)) return;
        if(playerData.GetLevel() < MinimumLevel) return;

        var hasDissipation = playerData.HasStatus(DissipationStatusId);
        
        dissipationDebouncer.Update(playerData.GetObjectId(), hasDissipation);
        if (dissipationDebouncer.IsLockedOut(playerData.GetObjectId())) return;

        if (!hasDissipation && !playerData.HasPet())
        {
            AddActiveWarning(SummonEosActionId, playerData);
        }
    }
}