namespace NoTankYou.Classes;

public class ConsumableModuleConfig : ConfigBase {
    public bool SuppressInCombat = true;
    public int EarlyWarningTime = 600;
    public bool ShowTimeRemaining;
    
    public bool SavageFilter;
    public bool UltimateFilter;
    public bool ExtremeUnrealFilter;
    public bool CriterionFilter;
    public bool ChaoticFilter;
}
