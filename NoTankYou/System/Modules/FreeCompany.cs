using System.Linq;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Abstracts;
using NoTankYou.Localization;
using NoTankYou.Models.Attributes;
using NoTankYou.Models.Enums;
using NoTankYou.Models.Interfaces;
using Condition = KamiLib.GameState.Condition;

namespace NoTankYou.System.Modules;

public class FreeCompanyConfiguration : ModuleConfigBase
{
    [Disabled]
    public new bool SoloMode = true;

    [Disabled]
    public new bool DutiesOnly = false;

    [Disabled]
    public new bool DisableInSanctuary = false;

    [EnumConfigOption("ModeSelect", "ModuleOptions", 1, "FreeCompanyModeHelp")]
    public FreeCompanyMode Mode = FreeCompanyMode.Any;

    [IntComboConfigOption("BuffCount", "ModuleOptions", 1, 1, 2, "FreeCompanyBuffCountHelp")]
    public int BuffCount = 2;
    
    [FreeCompanyStatusSelector("FirstBuff", "ModuleOptions", 1)]
    public uint PrimaryBuff = 360;

    [FreeCompanyStatusSelector("SecondBuff", "ModuleOptions", 1)]
    public uint SecondaryBuff = 364;
}

public class FreeCompany : ModuleBase
{
    public override ModuleName ModuleName => ModuleName.FreeCompany;
    public override string DefaultWarningText { get; protected set; } = Strings.FreeCompanyBuff;
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new FreeCompanyConfiguration();

    private const uint FreeCompanyActionId = 43;
    private readonly int freeCompanyIconId = LuminaCache<CompanyAction>.Instance.GetRow(FreeCompanyActionId)!.Icon;
    
    private readonly uint[] statusList = LuminaCache<Status>.Instance
        .Where(status => status.IsFcBuff)
        .Select(status => status.RowId)
        .ToArray();

    protected override bool ShouldEvaluate(IPlayerData playerData)
    {
        if (Condition.IsBoundByDuty()) return false;
        if (Service.ClientState.LocalPlayer?.ObjectId != playerData.GetObjectId()) return false;
        if (Service.ClientState.LocalPlayer?.HomeWorld.Id != Service.ClientState.LocalPlayer?.CurrentWorld.Id) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData)
    {
        var config = GetConfig<FreeCompanyConfiguration>();

        switch (config.Mode)
        {
            case FreeCompanyMode.Any when playerData.MissingStatus(statusList):
            case FreeCompanyMode.Specific when config.BuffCount is 1 && playerData.MissingStatus(config.PrimaryBuff):
            case FreeCompanyMode.Specific when config.BuffCount is 2 && playerData.MissingStatus(config.PrimaryBuff, config.SecondaryBuff):
                AddActiveWarning((uint)freeCompanyIconId, string.Empty, playerData);
                break;
        }
    }
}