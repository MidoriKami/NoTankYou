using System.Drawing;
using System.Text.Json.Serialization;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Configuration;
using NoTankYou.Localization;

namespace NoTankYou.Classes;

public abstract class ModuleConfigBase(ModuleName moduleName) {
	public bool Enabled;
	public bool SoloMode;
	public bool DutiesOnly = true;
	public bool DisableInSanctuary = true;
	public int Priority;
	public bool CustomWarning;
	public string CustomWarningText = string.Empty;
	public bool AutoSuppress;
	public int AutoSuppressTime = 60;

	[JsonIgnore] protected bool ConfigChanged { get; set; }
	[JsonIgnore] public virtual OptionDisableFlags OptionDisableFlags => OptionDisableFlags.None;

	public virtual void DrawConfigUi() {
		ImGuiTweaks.Header(Strings.General);
		using (var _ = ImRaii.PushIndent()) {
			ConfigChanged |= ImGui.Checkbox(Strings.Enable, ref Enabled);
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.SoloMode))) {
				ConfigChanged |= ImGuiTweaks.Checkbox(Strings.SoloMode, ref SoloMode, Strings.SoloModeHelp);
				DisabledSettingTooltip(disable);
			}
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.DutiesOnly))) {
				ConfigChanged |= ImGuiTweaks.Checkbox(Strings.DutiesOnly, ref DutiesOnly, Strings.DutiesOnlyHelp);
				DisabledSettingTooltip(disable);
			} 
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.Sanctuary))) {
				ConfigChanged |= ImGuiTweaks.Checkbox(Strings.HideInSanctuary, ref DisableInSanctuary, Strings.HideInSanctuaryHelp);
				DisabledSettingTooltip(disable);
			} 
            
			ConfigChanged |= ImGuiTweaks.PriorityInt(Service.PluginInterface, Strings.Priority, ref Priority);
		}

		if (!OptionDisableFlags.HasFlag(OptionDisableFlags.SoloMode)) {
			ImGuiTweaks.Header("Warning Suppression");
			using (ImRaii.PushIndent()) {
				ConfigChanged |= ImGuiTweaks.Checkbox("Auto Suppress", ref AutoSuppress, $"Automatically suppress warnings for other players after {AutoSuppressTime} Seconds");

				ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
				ConfigChanged |= ImGui.InputInt("Suppression Time (Seconds)", ref AutoSuppressTime, 0, 0);
			}
		}

		ImGuiTweaks.Header(Strings.DisplayOptions);
		using (var _ = ImRaii.PushIndent()) {
			ConfigChanged |= ImGui.Checkbox(Strings.CustomWarning, ref CustomWarning);
            
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ConfigChanged |= ImGui.InputTextWithHint("##Custom_Warning_Text", Strings.WarningText, ref CustomWarningText, 1024);
		} 
        
		ImGuiTweaks.Header(Strings.ModuleOptions);
		using (var _ = ImRaii.PushIndent()) {
			ImGuiHelpers.ScaledDummy(5.0f);
			DrawModuleConfig();
		}

		if (ConfigChanged) {
			Service.Log.Verbose($"Saving config for {moduleName}");
			Save();
			ConfigChanged = false;
		}
	}

	protected void DisabledSettingTooltip(ImRaii.IEndObject endObject) {
		if (endObject.Success && ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) {
			using var fullAlpha = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 1.0f);
			ImGui.SetTooltip("This setting is being ignored");
		}
	}

	protected virtual void DrawModuleConfig()
		=> ImGui.TextColored(KnownColor.Orange.Vector(), "No additional options for this module");

	public static T Load<T>(ModuleName moduleName) where T : new()
		=> Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, $"{moduleName}.config.json", () => new T());
	
	public void Save()
		=> Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, $"{moduleName}.config.json", this);
}