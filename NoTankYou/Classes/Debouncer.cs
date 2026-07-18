using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;

namespace NoTankYou.Classes;

public class Debouncer(TimeSpan delay) {
    private readonly HashSet<uint> characterWaitList = [];

    private bool lastState;

    public Debouncer() : this(TimeSpan.FromMilliseconds(750)) { }

    public void Update(uint objectId, bool state) {
        if (lastState && !state) {
            characterWaitList.Add(objectId);

            IFramework.Get().RunOnTick(() => {
                characterWaitList.Remove(objectId);
            }, delay);
        }

        lastState = state;
    }

    public bool IsLockedOut(uint objectId) => characterWaitList.Contains(objectId);
}
