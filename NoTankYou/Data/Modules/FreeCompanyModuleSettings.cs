using NoTankYou.Data.Components;
using NoTankYou.Enums;

namespace NoTankYou.Data.Modules
{
    public class FreeCompanyModuleSettings : GenericSettings
    {
        public FreeCompanyBuffScanMode ScanMode = FreeCompanyBuffScanMode.Any;
        public int BuffCount = 1;
        public uint[] BuffList = new uint[2];
    }
}
