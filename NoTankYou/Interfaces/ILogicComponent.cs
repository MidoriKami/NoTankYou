using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using NoTankYou.Configuration.Components;

namespace NoTankYou.Interfaces;

internal interface ILogicComponent
{
    IModule ParentModule { get; }
    List<uint> ClassJobs { get; }

    WarningState? EvaluateWarning(PlayerCharacter character);

    WarningState? ShouldShowWarning(PlayerCharacter character)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return null;
        if (ParentModule.GenericSettings.DutiesOnly.Value && !Service.DutyEventManager.DutyStarted) return null;
        if (ParentModule.GenericSettings.SoloMode.Value && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return null;
        if (character.CurrentHp == 0) return null;

        return EvaluateWarning(character);
    }
}