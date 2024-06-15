using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Components;
using KamiLib.Configuration;
using NoTankYou.Localization;

namespace NoTankYou.Classes;

public class SystemConfig : CharacterConfiguration {
	public bool WaitUntilDutyStart = true;
	public bool AutoSuppress;
	public int AutoSuppressTime = 60;

	public static SystemConfig Load() 
		=> Service.PluginInterface.LoadConfigFile("System.config.json", () => {
			var newConfig = new SystemConfig();
			newConfig.UpdateCharacterData(Service.ClientState);

			return newConfig;
		});

	private void Save()
		=> Service.PluginInterface.SaveConfigFile("System.config.json", this);

	public void DrawConfigUi() {
		var configChanged = ImGuiTweaks.Checkbox(Strings.WaitForDutyStart, ref WaitUntilDutyStart, Strings.WaitForDutyStartHelp);
		configChanged |= ImGuiTweaks.Checkbox("Auto Suppress", ref AutoSuppress, $"Automatically suppress warnings after {AutoSuppressTime} Seconds");
		
		ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
		configChanged |= ImGui.InputInt("Suppression Time (Seconds)", ref AutoSuppressTime, 0, 0);

		if (configChanged) {
			Save();
		}
	}
}