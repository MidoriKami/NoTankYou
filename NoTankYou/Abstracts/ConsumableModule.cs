using System.Collections.Generic;
using KamiLib.AutomaticUserInterface;
using KamiLib.GameState;
using KamiLib.Utilities;
using NoTankYou.Models.Interfaces;

namespace NoTankYou.Abstracts;

public class ConsumableConfig : ModuleConfigBase
{
    [BoolConfigOption("SuppressInCombat", "ModuleOptions", 1)]
    public bool SuppressInCombat = true;

    [IntCounterConfigOption("EarlyWarningTime", "ModuleOptions", 1, false)]
    public int EarlyWarningTime = 600;
    
    [BoolDescriptionConfigOption("EnableZoneFilter", "ZoneFilter", 2, "ZoneFilterDescription")]
    public bool ZoneFilter = false;

    [BoolConfigOption("Savage", "ZoneFilter", 2)]
    public bool SavageFilter = false;

    [BoolConfigOption("Ultimate", "ZoneFilter", 2)]
    public bool UltimateFilter = false;

    [BoolConfigOption("ExtremeUnreal", "ZoneFilter", 2)]
    public bool ExtremeUnrealFilter = false;

    [BoolConfigOption("Criterion", "ZoneFilter", 2)]
    public bool CriterionFilter = false;
}

public abstract class ConsumableModule : ModuleBase
{
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new ConsumableConfig();
    
    protected abstract uint IconId { get; set; }
    protected abstract string IconLabel { get; set; }
    protected abstract uint StatusId { get; set; }
    
    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        var config = GetConfig<ConsumableConfig>();
        
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
        if (playerData.GetStatusTimeRemaining(StatusId) < GetConfig<ConsumableConfig>().EarlyWarningTime)
        {
            AddActiveWarning(IconId, IconLabel, playerData);
        }
    }
}