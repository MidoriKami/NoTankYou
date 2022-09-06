using System;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using Condition = NoTankYou.Utilities.Condition;

namespace NoTankYou.System;

public unsafe class DutyEventManager : IDisposable
{
    private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);

    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
    private readonly Hook<DutyEventDelegate>? DutyEventHook = null;

    public bool DutyStarted { get; private set; }
    private bool CompletedThisTerritory;

    public DutyEventManager()
    {
        SignatureHelper.Initialise(this);

        DutyEventHook?.Enable();

        if (Condition.IsBoundByDuty())
        {
            DutyStarted = true;
        }

        Service.Framework.Update += FrameworkUpdate;
        Service.ClientState.TerritoryChanged += TerritoryChanged;
    }

    public void Dispose()
    {
        DutyEventHook?.Dispose();

        Service.Framework.Update -= FrameworkUpdate;
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
    }

    private void FrameworkUpdate(Framework framework)
    {
        if (!DutyStarted && !CompletedThisTerritory)
        {
            if (Condition.IsBoundByDuty() && Service.Condition[ConditionFlag.InCombat])
            {
                DutyStarted = true;
            }
        }
    }

    private void TerritoryChanged(object? sender, ushort e)
    {
        if (DutyStarted)
        {
            DutyStarted = false;
        }
            
        CompletedThisTerritory = false;
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
                        DutyStarted = true;
                        break;

                    // Party Wipe
                    case 0x40000005:
                        DutyStarted = false;
                        break;

                    // Duty Recommence
                    case 0x40000006:
                        DutyStarted = true;
                        break;

                    // Duty Completed
                    case 0x40000003:
                        DutyStarted = false;
                        CompletedThisTerritory = true;
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