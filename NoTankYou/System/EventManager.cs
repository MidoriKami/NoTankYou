using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using NoTankYou.Enums;
using NoTankYou.Utilities;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.System
{
    public unsafe class EventManager : IDisposable
    {
        private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);

        [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
        private readonly Hook<DutyEventDelegate>? DutyEventHook = null;

        public bool DutyStarted { get; private set; }

        public EventManager()
        {
            SignatureHelper.Initialise(this);

            DutyEventHook?.Enable();

            if (Condition.IsBoundByDuty())
            {
                Chat.Log(LogChannel.ContentDirector, "[PluginInit] Resetting Duty Started to {false}");
                DutyStarted = true;
            }

            Service.ClientState.TerritoryChanged += TerritoryChanged;
        }

        private void TerritoryChanged(object? sender, ushort e)
        {
            if (DutyStarted)
            {
                Chat.Log(LogChannel.ContentDirector, "[TerritoryChanged] Resetting Duty Started to {false}");
                DutyStarted = false;
            }
        }

        // Fallback listener that triggers duty started when combat starts while bound by duty
        public void Update()
        {
            if (!DutyStarted)
            {
                if (Condition.IsBoundByDuty())
                {
                    if (Service.Condition[ConditionFlag.InCombat] && !Service.Condition[ConditionFlag.BetweenAreas])
                    {
                        Chat.Log(LogChannel.ContentDirector, "[Update] Duty Commenced");
                        DutyStarted = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            DutyEventHook?.Dispose();

            Service.ClientState.TerritoryChanged -= TerritoryChanged;
        }

        private byte DutyEventFunction(void* a1, void* a2, ushort* a3)
        {
            try
            {
                var category = *(a3);
                var type = *(uint*)(a3 + 4);

                // DirectorUpdate Category
                if (category == 0x6D)
                {
                    switch (type)
                    {
                        // Duty Commenced
                        case 0x40000001:
                            Chat.Log(LogChannel.ContentDirector, $"[0x{type:x8}] Duty Commenced");
                            DutyStarted = true;
                            break;

                        // Party Wipe
                        case 0x40000005:
                            Chat.Log(LogChannel.ContentDirector, $"[0x{type:x8}] Party Wipe");
                            DutyStarted = false;
                            break;

                        // Duty Recommence
                        case 0x40000006:
                            Chat.Log(LogChannel.ContentDirector, $"[0x{type:x8}] Duty Recommenced");
                            DutyStarted = true;
                            break;

                        // Duty Completed
                        case 0x40000003:
                            Chat.Log(LogChannel.ContentDirector, $"[0x{type:x8}] Duty Completed");
                            DutyStarted = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to get Duty Started Status");
            }

            return DutyEventHook!.Original(a1, a2, a3);
        }
    }
}
