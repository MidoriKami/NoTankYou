using System.Collections.Generic;
using KamiLib.Game;
using NoTankYou.Models.Interfaces;
using NoTankYou.Models.ModuleConfiguration;

namespace NoTankYou.Abstracts;

public abstract class ConsumableModule : ModuleBase
{
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new ConsumableConfiguration();
    
    protected abstract uint IconId { get; set; }
    protected abstract string IconLabel { get; set; }
    protected abstract uint StatusId { get; set; }
    
    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        var config = GetConfig<ConsumableConfiguration>();
        
        if (config.SuppressInCombat && Condition.IsInCombat()) return false;

        if (config.SavageFilter || config.UltimateFilter || config.ExtremeUnrealFilter || config.CriterionFilter)
        {
            var allowedZones = new List<DutyType>();
            
            if(config.SavageFilter) allowedZones.Add(DutyType.Savage);
            if(config.UltimateFilter) allowedZones.Add(DutyType.Ultimate);
            if(config.ExtremeUnrealFilter) allowedZones.Add(DutyType.ExtremeUnreal);
            if(config.CriterionFilter) allowedZones.Add(DutyType.Criterion);

            if (!DutyLists.Instance.IsType(Service.ClientState.TerritoryType, allowedZones.ToArray())) return false;
        }
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        var statusTimeRemaining = playerData.GetStatusTimeRemaining(StatusId);
        
        if (statusTimeRemaining < GetConfig<ConsumableConfiguration>().EarlyWarningTime) {
            if (GetConfig<ConsumableConfiguration>() is { ShowTimeRemaining: true } && statusTimeRemaining is not 0) {
                ExtraWarningText = $" ({(int)playerData.GetStatusTimeRemaining(StatusId)}s)";
            }
            else {
                ExtraWarningText = string.Empty;
            }
            
            AddActiveWarning(IconId, IconLabel, playerData);
        }
    }
}