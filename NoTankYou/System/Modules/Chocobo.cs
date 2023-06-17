using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Abstracts;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using Condition = KamiLib.GameState.Condition;

namespace NoTankYou.System.Modules;

public class ChocoboConfiguration : ModuleConfigBase
{
    [Disabled]
    public new bool SoloMode = false;

    [Disabled]
    public new bool DutiesOnly = false;

    [BoolConfigOption("SuppressInCombat", "ModuleOptions", 1)]
    public bool DisableInCombat = true;

    [BoolConfigOption("EarlyWarning", "ModuleOptions", 1)]
    public bool EarlyWarning = true;
    
    [IntCounterConfigOption("EarlyWarningTime", "ModuleOptions", 1, false)]
    public int EarlyWarningTime = 300;
}

public unsafe class Chocobo : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Chocobo;
    public override string DefaultWarningText { get; protected set; } = "Chocobo Missing";
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new ChocoboConfiguration();

    private const uint GyshalGreensItemId = 4868;
    private readonly uint gysahlGreensIconId = LuminaCache<Item>.Instance.GetRow(GyshalGreensItemId)!.Icon;
    private readonly string gyshalGreensActionName = LuminaCache<Item>.Instance.GetRow(GyshalGreensItemId)!.Name.ToDalamudString().ToString();

    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (Condition.IsBoundByDuty()) return false;
        if (GetConfig<ChocoboConfiguration>().DisableInCombat && Condition.IsInCombat()) return false;
        if (playerData.GetObjectId() != Service.ClientState.LocalPlayer?.ObjectId) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        var config = GetConfig<ChocoboConfiguration>();

        var warningTime = config.EarlyWarning ? config.EarlyWarningTime : 0;

        if (UIState.Instance()->Buddy.TimeLeft <= warningTime)
        {
            AddActiveWarning(gysahlGreensIconId, gyshalGreensActionName, playerData);
        }
    }
}