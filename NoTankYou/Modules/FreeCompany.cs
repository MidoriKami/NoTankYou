using System;
using System.Linq;
using System.Text.Json.Serialization;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using KamiLib.Classes;
using KamiLib.Extensions;
using KamiLib.Window;
using Lumina.Excel.Sheets;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;
using NoTankYou.Windows;

namespace NoTankYou.Modules;

public class FreeCompany : ModuleBase<FreeCompanyConfiguration> {
    public override ModuleName ModuleName => ModuleName.FreeCompany;
    protected override string DefaultWarningText => "Free Company Buff";

    private const uint FreeCompanyActionId = 43;
    private readonly int freeCompanyIconId = Services.DataManager.GetExcelSheet<CompanyAction>().GetRow(FreeCompanyActionId).Icon;
    
    private readonly uint[] statusList = Services.DataManager.GetExcelSheet<Status>()
        .Where(status => status.IsFcBuff)
        .Select(status => status.RowId)
        .ToArray();

    protected override bool ShouldEvaluate(IPlayerData playerData) {
        if (Services.Condition.IsBoundByDuty()) return false;
        if (Services.ObjectTable.LocalPlayer?.EntityId != playerData.GetEntityId()) return false;
        if (Services.ObjectTable.LocalPlayer?.HomeWorld.RowId != Services.ObjectTable.LocalPlayer?.CurrentWorld.RowId) return false;

        return true;
    }
    
    protected override void EvaluateWarnings(IPlayerData playerData) {
        switch (Config.Mode) {
            case FreeCompanyMode.Any when playerData.MissingStatus(statusList):
            case FreeCompanyMode.Specific when Config.BuffCount is 1 && Config.PrimaryBuff is not 0 ? playerData.MissingStatus(Config.PrimaryBuff) : playerData.MissingStatus(Config.SecondaryBuff):
            case FreeCompanyMode.Specific when Config.BuffCount is 2 && (playerData.MissingStatus(Config.PrimaryBuff) || playerData.MissingStatus(Config.SecondaryBuff)):
                AddActiveWarning((uint)freeCompanyIconId, string.Empty, playerData);
                break;
        }
    }
}

public enum FreeCompanyMode {
    Any,
    Specific,
}

public class FreeCompanyConfiguration() : ModuleConfigBase(ModuleName.FreeCompany) {
    public override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.Sanctuary | OptionDisableFlags.DutiesOnly | OptionDisableFlags.SoloMode;
    
    public FreeCompanyMode Mode = FreeCompanyMode.Any;
    public uint PrimaryBuff;
    public uint SecondaryBuff;

    [JsonIgnore] public int BuffCount => this switch {
        { PrimaryBuff: not 0, SecondaryBuff: 0 } => 1,
        { PrimaryBuff: 0, SecondaryBuff: not 0 } => 1,
        { PrimaryBuff: not 0, SecondaryBuff: not 0 } => 2,
        _ => 0,
    };

    // todo: this is kinda messy? Maybe rework it entirely?
    protected override void DrawModuleConfig() {
        ImGui.SetNextItemWidth(100.0f * ImGuiHelpers.GlobalScale);
        ConfigChanged |= ImGuiTweaks.EnumCombo("Mode Select", ref Mode);

        // Any mode doesn't care about Primary/Secondary
        if (Mode is FreeCompanyMode.Any) return;
        
        ImGuiHelpers.ScaledDummy(3.0f);
        if (PrimaryBuff is 0) {
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Plus, "PrimaryBuffSelect", ImGuiHelpers.ScaledVector2(150.0f, 23.0f))) {
                OpenPrimaryStatusSelect();
            }
            
            ImGui.SameLine();
            ImGui.Text("First Buff");
        }
        else {
            var status = Services.DataManager.GetExcelSheet<Status>().GetRow(PrimaryBuff);
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.ExchangeAlt, "ChangePrimaryBuff", ImGuiHelpers.ScaledVector2(24.0f, 24.0f))) {
                OpenPrimaryStatusSelect();
            }
            
            ImGui.SameLine();
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Trash, "PrimaryReset", ImGuiHelpers.ScaledVector2(24.0f, 24.0f))) {
                PrimaryBuff = 0;
                ConfigChanged = true;
            }
            
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);

            ImGui.Image(Services.TextureProvider.GetFromGameIcon(status.Icon).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(24.0f, 32.0f));
            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.Text(status.Name.ToString());
        }
        
        if (SecondaryBuff is 0) {
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Plus, "SecondaryBuffSelect", ImGuiHelpers.ScaledVector2(150.0f, 23.0f))) {
                OpenSecondaryStatusSelect();
            }
            
            ImGui.SameLine();
            ImGui.Text("Second Buff");
        }
        else {
            var status = Services.DataManager.GetExcelSheet<Status>().GetRow(SecondaryBuff);
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.ExchangeAlt, "ChangeSecondaryBuff", ImGuiHelpers.ScaledVector2(24.0f, 24.0f))) {
                OpenSecondaryStatusSelect();
            }

            ImGui.SameLine();
            if (ImGuiTweaks.IconButtonWithSize(Services.PluginInterface.UiBuilder.IconFontFixedWidthHandle, FontAwesomeIcon.Trash, "SecondaryReset", ImGuiHelpers.ScaledVector2(24.0f, 24.0f))) {
                SecondaryBuff = 0;
                ConfigChanged = true;
            }
            
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);

            ImGui.Image(Services.TextureProvider.GetFromGameIcon(status.Icon).GetWrapOrEmpty().Handle, ImGuiHelpers.ScaledVector2(24.0f, 32.0f));
            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.Text(status.Name.ToString());
        }
    }

    private void OpenPrimaryStatusSelect()
        => OpenStatusSelect(result => {
            if (result.RowId is not 0) {
                PrimaryBuff = result.RowId;
                ConfigChanged = true;
            }
        });

    private void OpenSecondaryStatusSelect()
        => OpenStatusSelect(result => {
            if (result.RowId is not 0) {
                SecondaryBuff = result.RowId;
                ConfigChanged = true;
            }
        });

    private void OpenStatusSelect(Action<Status> callback) {
        System.WindowManager.AddWindow(new FreeCompanyStatusSelectionWindow {
            SingleSelectionCallback = callback,
        }, WindowFlags.OpenImmediately);
    }
}