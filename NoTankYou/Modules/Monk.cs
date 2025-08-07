using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Interface.Utility;
using KamiLib.Extensions;
using NoTankYou.Classes;
using NoTankYou.PlayerDataInterface;

namespace NoTankYou.Modules;

public class Monk : ModuleBase<MonkConfiguration> {
	public override ModuleName ModuleName => ModuleName.Monk;
	protected override string DefaultWarningText => "Monk Warning";

	private const byte MinimumLevel = 40;
	private const byte MonkClassJob = 20;
	
	private const uint MantraActionId = 36943;
	private const uint MantraMinimumLevel = 54;
	
	private const uint FormlessFistActionId = 4262;
	private const uint FormlessFistStatusEffect = 2513;
	private const uint FormlessFistMinimumLevel = 52;

	private DateTime lastCombatTime = DateTime.UtcNow;
	
	protected override bool ShouldEvaluate(IPlayerData playerData) {
		if (Service.ClientState.LocalPlayer?.EntityId != playerData.GetEntityId()) return false;
		if (!playerData.HasClassJob(MonkClassJob)) return false;
		if (playerData.GetLevel() < MinimumLevel) return false;

		return true;
	}

	protected override void EvaluateWarnings(IPlayerData playerData) {
		if (Service.Condition.IsInCombat()) {
			lastCombatTime = DateTime.UtcNow;
		}

		if (DateTime.UtcNow - lastCombatTime > TimeSpan.FromSeconds(Config.WarningDelay)) {
			
			// Mantra
			if (playerData.GetLevel() >= MantraMinimumLevel) {
				if (Service.JobGauges.Get<MNKGauge>().Chakra < 5) {
					AddActiveWarning(MantraActionId, playerData);
					return;
				}
			}
			
			// Formless Fist
			if (playerData.GetLevel() >= FormlessFistMinimumLevel) {
				if (Config.FormlessFist && playerData.MissingStatus(FormlessFistStatusEffect)) {
					AddActiveWarning(FormlessFistActionId, playerData);
					return;
				}
			}
		}
	}
}

public class MonkConfiguration() : ModuleConfigBase(ModuleName.Monk) {
	public override OptionDisableFlags OptionDisableFlags => OptionDisableFlags.SoloMode;

	public int WarningDelay = 5;

	public bool FormlessFist;

	protected override void DrawModuleConfig() {
		ConfigChanged |= ImGui.Checkbox("Formless Fist Warning", ref FormlessFist);
		
		ImGuiHelpers.ScaledDummy(5.0f);
		
		ImGui.PushItemWidth(50.0f * ImGuiHelpers.GlobalScale);
		ConfigChanged |= ImGui.InputInt("Warning Delay Time (seconds)", ref WarningDelay, 0, 0);
	}
}
