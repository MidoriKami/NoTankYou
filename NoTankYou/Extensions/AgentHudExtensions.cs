using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace NoTankYou.Extensions;

public static class AgentHudExtensions {
	public static HudPartyMember? GetMember(ref this AgentHUD agentHud, ulong entityId) {
		foreach (var member in agentHud.PartyMembers) {
			if (member.EntityId == entityId) {
				return member;
			}
		}

		return null;
	}
}