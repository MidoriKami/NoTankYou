using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Classes;
using KamiLib.Configuration;
using NoTankYou.Localization;

namespace NoTankYou.Classes;

public class SystemConfig : CharacterConfiguration {
	public bool WaitUntilDutyStart = true;
	public bool AutoSuppress;
	public int AutoSuppressTime = 60;

	public bool HideInQuestEvent = true;

	public static SystemConfig Load() 
		=> Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "System.config.json", () => {
			var newConfig = new SystemConfig();
			newConfig.UpdateCharacterData(Service.ClientState);

			return newConfig;
		});

	public void Save()
		=> Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId,"System.config.json", this);

	public void DrawConfigUi() {
		var configChanged = false;
		
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Misc Options");
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= ImGuiTweaks.Checkbox(Strings.WaitForDutyStart, ref WaitUntilDutyStart, Strings.WaitForDutyStartHelp);
		}
		
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Warning Suppression");
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= ImGuiTweaks.Checkbox("Auto Suppress", ref AutoSuppress, $"Automatically suppress warnings for other players after {AutoSuppressTime} Seconds");
		
			ImGuiHelpers.ScaledDummy(5.0f);

			ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
			configChanged |= ImGui.InputInt("Suppression Time (Seconds)", ref AutoSuppressTime, 0, 0);
		}
		
		ImGuiHelpers.ScaledDummy(10.0f);
		ImGui.Text("Warning Display Options");
		ImGui.Separator();
		ImGuiHelpers.ScaledDummy(5.0f);
		using (ImRaii.PushIndent()) {
			configChanged |= ImGui.Checkbox("Hide in Quest Event", ref HideInQuestEvent);
		}

		if (configChanged) {
			Save();
		}
	}
}