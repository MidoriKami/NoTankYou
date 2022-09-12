using System;
using NoTankYou.Configuration.Components;
using NoTankYou.Localization;

namespace NoTankYou.Configuration.ModuleSettings;

public enum FreeCompanyBuffScanMode
{
    Any,
    Specific
}

public class FreeCompanyConfiguration : GenericSettings
{
    public Setting<FreeCompanyBuffScanMode> ScanMode = new(FreeCompanyBuffScanMode.Any);
    public Setting<int> BuffCount = new(1);
    public uint[] BuffList = new uint[2];
}

public static class FreeCompanyBuffScanModeExtensions
{
    public static string GetLabel(this FreeCompanyBuffScanMode mode)
    {
        return mode switch
        {
            FreeCompanyBuffScanMode.Any => Strings.Modules.FreeCompany.Any,
            FreeCompanyBuffScanMode.Specific => Strings.Modules.FreeCompany.Specific,

            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}