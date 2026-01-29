using System.Collections.Generic;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;

namespace NoTankYou.Extensions;

public static class CommandManagerExtensions {
	extension(ICommandManager commandManager) {
		public void AddHandler(ICollection<string> commands, CommandInfo commandInfo) {
			foreach (var command in commands) {
				commandManager.AddHandler(command, commandInfo);
			}
		}
	}
}