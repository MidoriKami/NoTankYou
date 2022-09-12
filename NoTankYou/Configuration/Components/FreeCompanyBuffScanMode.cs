using System;
using NoTankYou.Localization;

namespace NoTankYou.Configuration.Components;

public enum FreeCompanyBuffScanMode
{
    Any,
    Specific
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