using System.Collections.Generic;
using KamiLib.AutomaticUserInterface;
using KamiLib.GameState;
using KamiLib.Utilities;
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

        if (config.ZoneFilter)
        {
            var allowedZones = new List<DutyType>();
            
            if(config.SavageFilter) allowedZones.Add(DutyType.Savage);
            if(config.UltimateFilter) allowedZones.Add(DutyType.Ultimate);
            if(config.ExtremeUnrealFilter) allowedZones.Add(DutyType.ExtremeUnreal);
            if(config.CriterionFilter) allowedZones.Add(DutyType.Criterion);

            if (!DutyLists.Instance.IsType(Service.ClientState.TerritoryType, allowedZones)) return false;
        }
        
        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        if (playerData.GetStatusTimeRemaining(StatusId) < GetConfig<ConsumableConfiguration>().EarlyWarningTime)
        {
            AddActiveWarning(IconId, IconLabel, playerData);
        }
    }
}