using System;

namespace NoTankYou.Classes;

[Flags]
public enum OptionDisableFlags {
	None = 0,
	SoloMode = 1 << 1,
	DutiesOnly = 1 << 2,
	Sanctuary = 1 << 3,
}