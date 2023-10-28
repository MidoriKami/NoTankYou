using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using NoTankYou.Models.ModuleConfiguration;
using Condition = KamiLib.Game.Condition;

namespace NoTankYou.System.Modules;

public unsafe class Chocobo : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.Chocobo;
    protected override string DefaultWarningText { get; } = Strings.ChocoboMissing;
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new ChocoboConfiguration();

    private const uint GyshalGreensItemId = 4868;
    private readonly uint gysahlGreensIconId = LuminaCache<Item>.Instance.GetRow(GyshalGreensItemId)!.Icon;
    private readonly string gyshalGreensActionName = LuminaCache<Item>.Instance.GetRow(GyshalGreensItemId)!.Name.ToDalamudString().ToString();

    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (GameMain.IsInSanctuary() && ActionManager.Instance()->GetActionStatus(ActionType.Item, 4868) is not 0) return false;
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