using System.Drawing;
using System.Text.Json.Serialization;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;
using KamiLib.Configuration;

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

	public void DrawConfigUi() {
		ImGuiTweaks.Header("General");
		using (var _ = ImRaii.PushIndent()) {
			ConfigChanged |= ImGui.Checkbox("Enable", ref Enabled);
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.SoloMode))) {
				ConfigChanged |= ImGuiTweaks.Checkbox("Solo Mode", ref SoloMode, "Only generate warnings for you");
				DisabledSettingTooltip(disable);
			}
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.DutiesOnly))) {
				ConfigChanged |= ImGuiTweaks.Checkbox("Duties Only", ref DutiesOnly, "Only generate warnings while in a duty");
				DisabledSettingTooltip(disable);
			} 
            
			using (var disable = ImRaii.Disabled(OptionDisableFlags.HasFlag(OptionDisableFlags.Sanctuary))) {
				ConfigChanged |= ImGuiTweaks.Checkbox("Hide in Sanctuary", ref DisableInSanctuary, "Prevents warnings from showing while you are in a sanctuary");
				DisabledSettingTooltip(disable);
			} 
            
			ConfigChanged |= ImGuiTweaks.PriorityInt(Services.PluginInterface, "Priority", ref Priority);
		}

		if (!OptionDisableFlags.HasFlag(OptionDisableFlags.SoloMode)) {
			ImGuiTweaks.Header("Warning Suppression");
			using (ImRaii.PushIndent()) {
				ConfigChanged |= ImGuiTweaks.Checkbox("Auto Suppress", ref AutoSuppress, $"Automatically suppress warnings for other players after {AutoSuppressTime} Seconds");

				ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
				ConfigChanged |= ImGui.InputInt("Suppression Time (Seconds)", ref AutoSuppressTime);
			}
		}

		ImGuiTweaks.Header("Display Options");
		using (var _ = ImRaii.PushIndent()) {
			ConfigChanged |= ImGui.Checkbox("Custom Warning", ref CustomWarning);
            
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ConfigChanged |= ImGui.InputTextWithHint("##Custom_Warning_Text", "Warning Text", ref CustomWarningText, 1024);
		} 
        
		ImGuiTweaks.Header("Module Options");
		using (var _ = ImRaii.PushIndent()) {
			ImGuiHelpers.ScaledDummy(5.0f);
			DrawModuleConfig();
		}

		if (ConfigChanged) {
			Services.PluginLog.Verbose($"Saving config for {moduleName}");
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
		=> Services.PluginInterface.LoadCharacterFile<T>(Services.PlayerState.ContentId, $"{moduleName}.config.json");
	
	public void Save()
		=> Services.PluginInterface.SaveCharacterFile(Services.PlayerState.ContentId, $"{moduleName}.config.json", this);
}