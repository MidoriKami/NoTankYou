using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using NoTankYou.Components;
using NoTankYou.Data.Components;

namespace NoTankYou.Interfaces
{
    public interface IModule : ICommand
    {
        List<uint> ClassJobs { get; }
        GenericSettings GenericSettings { get; }
        string MessageLong { get; }
        string MessageShort { get; }
        string ModuleCommand { get; }

        WarningState? EvaluateWarning(PlayerCharacter character);

        WarningState? ShouldShowWarning(PlayerCharacter character)
        {
            if (!GenericSettings.Enabled) return null;
            if (GenericSettings.DutiesOnly && !Service.EventManager.DutyStarted) return null;
            if (GenericSettings.SoloMode && character.ObjectId != Service.ClientState.LocalPlayer?.ObjectId) return null;
            if (!Service.HudManager.IsTargetable(character.ObjectId)) return null;
            if (character.CurrentHp == 0) return null;

            return EvaluateWarning(character);
        }
        
        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == ModuleCommand)
            {
                switch (secondaryCommand)
                {
                    case null:
                        GenericSettings.Enabled = !GenericSettings.Enabled;
                        break;

                    case "on":
                        GenericSettings.Enabled = true;
                        break;

                    case "off":
                        GenericSettings.Enabled = false;
                        break;
                }
            }
        }
    }
}
