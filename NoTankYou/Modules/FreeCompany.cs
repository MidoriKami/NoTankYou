using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.DataModels;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules;

public class FreeCompanyConfiguration : GenericSettings
{
    public Setting<FreeCompanyBuffScanMode> ScanMode = new(FreeCompanyBuffScanMode.Any);
    public Setting<int> BuffCount = new(1);
    public uint[] BuffList = new uint[2];
}

public class FreeCompany : BaseModule
{
    public override ModuleName Name => ModuleName.FreeCompany;
    public override string Command => "fc";
    public override List<uint> ClassJobs { get; }

    private static FreeCompanyConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.FreeCompany;
    public override GenericSettings GenericSettings => Settings;

    private readonly List<uint> freeCompanyStatusIDList;
    private readonly CompanyAction freeCompanyStatus;
    
    public FreeCompany()
    {
        Settings.SoloMode.Value = true;
        Settings.DutiesOnly.Value = false;

        ClassJobs = LuminaCache<ClassJob>.Instance
            .Select(r => r.RowId)
            .ToList();

        freeCompanyStatusIDList = LuminaCache<Status>.Instance
            .Where(status => status.IsFcBuff)
            .Select(status => status.RowId)
            .ToList();

        freeCompanyStatus = LuminaCache<CompanyAction>.Instance.GetRow(43)!;
    }

    public  override WarningState? EvaluateWarning(PlayerCharacter character)
    {
        if (Service.DutyState.IsDutyStarted) return null;
        if (!CurrentlyInHomeworld(character)) return null;

        switch (Settings.ScanMode.Value)
        {
            case FreeCompanyBuffScanMode.Any:
                var fcBuffCount = character.StatusCount(freeCompanyStatusIDList);
                if (fcBuffCount < Settings.BuffCount.Value)
                {
                    return new WarningState
                    {
                        MessageLong = Strings.FreeCompany_WarningText,
                        MessageShort = Strings.FreeCompany_WarningTextShort,
                        IconID = (uint)freeCompanyStatus.Icon,
                        IconLabel = "",
                        Priority = Settings.Priority.Value,
                    };
                }
                break;

            case FreeCompanyBuffScanMode.Specific:
                for (var i = 0; i < Settings.BuffCount.Value; ++i)
                {
                    var targetStatus = LuminaCache<Status>.Instance.GetRow(Settings.BuffList[i])!;

                    if (!character.HasStatus(targetStatus.RowId))
                    {
                        return new WarningState
                        {
                            MessageLong = Strings.FreeCompany_WarningText,
                            MessageShort = Strings.FreeCompany_WarningTextShort,
                            IconID = (uint)freeCompanyStatus.Icon,
                            IconLabel = targetStatus.Name.RawString,
                            Priority = Settings.Priority.Value,
                        };
                    }
                }
                break;
        }

        return null;
    }

    private static bool CurrentlyInHomeworld(PlayerCharacter character)
    {
        var homeworld = character.HomeWorld.Id;
        var currentWorld = character.CurrentWorld.Id;

        return homeworld == currentWorld;
    }

    private void BuffSelection()
    {
        for (var i = 0; i < Settings.BuffCount.Value; i++)
        {
            var displayValue = Strings.Labels_Unset;
            if (Settings.BuffList[i] != 0)
            {
                var status = LuminaCache<Status>.Instance.GetRow(Settings.BuffList[i]);

                if (status != null)
                {
                    displayValue = status.Name.RawString;
                }
            }

            ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
            if (ImGui.BeginCombo($"###BuffSelection{i}", displayValue))
            {
                foreach (var status in LuminaCache<Status>.Instance.Where(status => status.IsFcBuff).OrderBy(s => s.Name.RawString))
                {
                    if (ImGui.Selectable(status.Name.RawString, status.RowId == Settings.BuffList[i]))
                    {
                        Settings.BuffList[i] = status.RowId;
                        Service.ConfigurationManager.Save();
                    }
                }

                ImGui.EndCombo();
            }
        }
    }

    private void DrawBuffCount()
    {
        ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
        if (ImGui.BeginCombo("###BuffCountCombo", Settings.BuffCount.ToString()))
        {
            if (ImGui.Selectable("1", Settings.BuffCount == 1))
            {
                Settings.BuffCount.Value = 1;
                Service.ConfigurationManager.Save();
            }

            if (ImGui.Selectable("2", Settings.BuffCount == 2))
            {
                Settings.BuffCount.Value = 2;
                Service.ConfigurationManager.Save();
            }

            ImGui.EndCombo();
        }
    }
    
    protected override void DrawExtraConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Labels_ModeSelect)
            .BeginTable()
            .BeginRow()
            .AddConfigRadio(Strings.FreeCompany_Any, Settings.ScanMode, FreeCompanyBuffScanMode.Any)
            .AddConfigRadio(Strings.FreeCompany_Specific, Settings.ScanMode, FreeCompanyBuffScanMode.Specific)
            .EndRow()
            .EndTable()
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.FreeCompany_BuffCount)
            .AddAction(DrawBuffCount)
            .Draw();

        if (Settings.ScanMode == FreeCompanyBuffScanMode.Specific)
        {
            InfoBox.Instance
                .AddTitle(Strings.FreeCompany_BuffSelection)
                .AddAction(BuffSelection)
                .Draw();
        }
    }
}