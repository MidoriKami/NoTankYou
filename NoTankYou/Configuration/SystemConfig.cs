using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Classes;
using KamiLib.Configuration;
using NoTankYou.Localization;

namespace NoTankYou.Configuration;

public unsafe class SystemConfig : CharacterConfiguration {
	public bool WaitUntilDutyStart = true;
	public bool HideInQuestEvent = true;

	public static SystemConfig Load() {
		var config = Service.PluginInterface.LoadCharacterFile<SystemConfig>(PlayerState.Instance()->ContentId, "System.config.json");

		if (config.ContentId is 0) {
			config.UpdateCharacterData();
			config.Save();
		}
		
		return config;
	}

	public void Save()
		=> Service.PluginInterface.SaveCharacterFile(PlayerState.Instance()->ContentId,"System.config.json", this);

	public void DrawConfigUi() {
		var configChanged = false;
		
		ImGuiTweaks.Header("Misc Options");
		using (ImRaii.PushIndent()) {
			configChanged |= ImGuiTweaks.Checkbox(Strings.WaitForDutyStart, ref WaitUntilDutyStart, Strings.WaitForDutyStartHelp);
		}
		
		ImGuiTweaks.Header("Warning Display Options");
		using (ImRaii.PushIndent()) {
			configChanged |= ImGui.Checkbox("Hide in Quest Event", ref HideInQuestEvent);
		}

		if (configChanged) {
			Save();
		}
	}
}