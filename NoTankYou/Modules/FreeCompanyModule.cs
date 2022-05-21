using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Modules
{
    internal class FreeCompanyModule : IModule
    {
        public List<uint> ClassJobs { get; }
        public GenericSettings GenericSettings => Settings;
        public string WarningText => Strings.Modules.FreeCompany.WarningText;
        private static FreeCompanyModuleSettings Settings => Service.Configuration.ModuleSettings.FreeCompany;

        private readonly List<uint> FreeCompanyStatusIDList;

        public FreeCompanyModule()
        {
            Settings.SoloMode = true;
            Settings.DutiesOnly = false;

            ClassJobs = Service.DataManager.GetExcelSheet<ClassJob>()!
                .Select(r => r.RowId)
                .ToList();

            FreeCompanyStatusIDList = Service.DataManager.GetExcelSheet<Status>()!
                .Where(status => status.IsFcBuff)
                .Select(status => status.RowId)
                .ToList();
        }

        public bool EvaluateWarning(PlayerCharacter character)
        {
            switch (Settings.ScanMode)
            {
                case FreeCompanyBuffScanMode.Any:
                    var fcBuffCount = character.StatusList.Count(status => FreeCompanyStatusIDList.Contains(status.StatusId));
                    return fcBuffCount < Settings.BuffCount;

                case FreeCompanyBuffScanMode.Specific:
                    for (var i = 0; i < Settings.BuffCount; ++i)
                    {
                        var targetStatus = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i])!.RowId;

                        // If this status is missing, send warning
                        if (!character.StatusList.Any(status => status.StatusId == targetStatus))
                        {
                            return true;
                        }
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
