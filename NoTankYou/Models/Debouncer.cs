using System;
using System.Collections.Generic;

namespace NoTankYou.Models;

public class Debouncer
{
    private readonly HashSet<uint> characterWaitList = new();
    private readonly TimeSpan delayTime;

    private bool lastState;

    public Debouncer(TimeSpan delay) => delayTime = delay;
    public Debouncer() => delayTime = TimeSpan.FromMilliseconds(500);

    public void Update(uint objectId, bool state)
    {
        if (lastState && !state)
        {
            characterWaitList.Add(objectId);

            Service.Framework.RunOnTick(() => {
                characterWaitList.Remove(objectId);
            }, delayTime);
        }

        lastState = state;
    }

    public bool IsLockedOut(uint objectId) => characterWaitList.Contains(objectId);
}