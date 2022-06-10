using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
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
                DutyStarted = true;
            }

            Service.ClientState.TerritoryChanged += TerritoryChanged;
        }

        private void TerritoryChanged(object? sender, ushort e)
        {
            DutyStarted = false;
        }

        // Fallback listener that triggers duty started when combat starts while bound by duty
        public void Update()
        {
            if (Condition.IsBoundByDuty())
            {
                if (!DutyStarted && Service.Condition[ConditionFlag.InCombat])
                {
                    DutyStarted = true;
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
                    DutyStarted = type switch
                    {
                        // Duty Commenced
                        0x40000001 => true,

                        // Party Wipe
                        0x40000005 => false,

                        // Duty Recommence
                        0x40000006 => true,

                        // Duty Completed
                        0x40000003 => false,
                        _ => DutyStarted
                    };
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
