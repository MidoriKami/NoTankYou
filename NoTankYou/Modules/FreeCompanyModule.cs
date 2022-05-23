using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class FreeCompanyModule : IModule
    {
        public List<uint> ClassJobs { get; }
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.FreeCompany.WarningText;
        public string MessageShort => Strings.Modules.FreeCompany.WarningTextShort;
        public string ModuleCommand => "fc";
        private static FreeCompanyModuleSettings Settings => Service.Configuration.ModuleSettings.FreeCompany;

        private readonly List<uint> FreeCompanyStatusIDList;

        private readonly CompanyAction FreeCompanyStatus;

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

            FreeCompanyStatus = Service.DataManager.GetExcelSheet<CompanyAction>()!.GetRow(43)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Service.EventManager.DutyStarted) return null;

            switch (Settings.ScanMode)
            {
                case FreeCompanyBuffScanMode.Any:
                    var fcBuffCount = character.StatusList.Count(status => FreeCompanyStatusIDList.Contains(status.StatusId));
                    if (fcBuffCount < Settings.BuffCount)
                    {
                        return new WarningState
                        {
                            MessageLong = MessageLong,
                            MessageShort = MessageShort,
                            IconID = (uint)FreeCompanyStatus.Icon,
                            IconLabel = "",
                            Priority = Settings.Priority
                        };
                    }
                    break;

                case FreeCompanyBuffScanMode.Specific:
                    for (var i = 0; i < Settings.BuffCount; ++i)
                    {
                        var targetStatus = Service.DataManager.GetExcelSheet<Status>()!.GetRow(Settings.BuffList[i])!;

                        if (!character.StatusList.Any(status => status.StatusId == targetStatus.RowId))
                        {
                            return new WarningState
                            {
                                MessageLong = MessageLong,
                                MessageShort = MessageShort,
                                IconID = (uint)FreeCompanyStatus.Icon,
                                IconLabel = targetStatus.Name.RawString,
                                Priority = Settings.Priority
                            };
                        }
                    }
                    break;
            }

            return null;
        }
    }
}
