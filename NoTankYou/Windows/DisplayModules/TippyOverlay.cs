using System;
using System.Diagnostics;
using System.Linq;
using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using Dalamud.Utility.Signatures;
using NoTankYou.Components;
using NoTankYou.Data.Overlays;
using NoTankYou.Enums;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.Windows.DisplayModules
{
    internal class TippyOverlay : IDisposable
    {
        private readonly ICallGateSubscriber<string, bool> TippyRegisterMessage;
        private static TippyOverlaySettings Settings => Service.Configuration.DisplaySettings.TippyOverlay;
        
        private readonly Stopwatch TipStopwatch = Stopwatch.StartNew();

        private delegate bool IsInSanctuary();

        [Signature("E8 ?? ?? ?? ?? 84 C0 75 21 48 8B 4F 10")]
        private readonly IsInSanctuary SanctuaryFunction = null!;

        public TippyOverlay()
        {
            SignatureHelper.Initialise(this);

            TippyRegisterMessage = Service.PluginInterface.GetIpcSubscriber<string, bool>("Tippy.RegisterMessage");
            Service.Framework.Update += UpdateTippyOverlay;
        }

        private void UpdateTippyOverlay(Framework framework)
        {
            if (!TippyInstalled()) return;
            if (!Condition.ShouldShowWarnings()) return;
            if (!Settings.Enabled) return;
            if (Settings.DisableInSanctuary && SanctuaryFunction()) return;
            if (TipStopwatch.Elapsed.Seconds < Settings.WarningFrequency) return;
            if (Settings.DutiesOnly && !Service.EventManager.DutyStarted) return;

            try
            {
                SendWarning();
            }
            catch (IpcNotReadyError error)
            {
                PluginLog.Error(error.Message);
            }
            finally
            {
                TipStopwatch.Restart();
            }
        }

        private void SendWarning()
        {
            if (Settings.SoloMode)
            {
                var warning = Service.HudManager.WarningStates[0];
                if (warning == null) return;

                InvokeForWarning(warning);
            }
            else
            {
                var warning = Service.HudManager.WarningStates.FirstOrDefault(warning => warning != null);

                if (warning != null) InvokeForWarning(warning);
            }
        }

        private void InvokeForWarning(WarningState warning)
        {
            var message = GetMessageForWarning(warning);

            if (message != string.Empty)
            {
                // Send Message Twice to display for 10 seconds
                TippyRegisterMessage.InvokeFunc(message);
                TippyRegisterMessage.InvokeFunc(message);
            }
        }

        private string GetMessageForWarning(WarningState warning)
        {
            var random = new Random().Next();
            int index;

            switch (warning.Sender)
            {
                case ModuleType.Tanks:
                    index = random % Strings.Tippy.Tank.Count;
                    return Strings.Tippy.Tank[index];

                case ModuleType.Dancer:
                    index = random % Strings.Tippy.Dancer.Count;
                    return Strings.Tippy.Dancer[index];

                case ModuleType.Food:
                    index = random % Strings.Tippy.Food.Count;
                    return Strings.Tippy.Food[index];

                case ModuleType.Sage:
                    index = random % Strings.Tippy.Sage.Count;
                    return Strings.Tippy.Sage[index];

                case ModuleType.Scholar:
                    index = random % Strings.Tippy.Scholar.Count;
                    return Strings.Tippy.Scholar[index];

                case ModuleType.Summoner:
                    index = random % Strings.Tippy.Summoner.Count;
                    return Strings.Tippy.Summoner[index];

                case ModuleType.BlueMage:
                    switch (warning.SenderSubtype)
                    {
                        case SenderSubtype.AetherialMimicry:
                            index = random % Strings.Tippy.BlueMage.AetherialMimicry.Count;
                            return Strings.Tippy.BlueMage.AetherialMimicry[index];

                        case SenderSubtype.MightyGuard:
                            index = random % Strings.Tippy.BlueMage.MightyGuard.Count;
                            return Strings.Tippy.BlueMage.AetherialMimicry[index];

                        case SenderSubtype.BasicInstinct:
                            index = random % Strings.Tippy.BlueMage.BasicInstinct.Count;
                            return Strings.Tippy.BlueMage.AetherialMimicry[index];
                    }
                    break;

                case ModuleType.FreeCompany:
                    index = random % Strings.Tippy.FreeCompany.Count;
                    return Strings.Tippy.FreeCompany[index];
            }

            return string.Empty;
        }

        public void Dispose()
        {
            Service.Framework.Update -= UpdateTippyOverlay;
        }

        private bool TippyInstalled()
        {
            return Service.PluginInterface.PluginInternalNames.Contains("Tippy");
        }
    }
}
