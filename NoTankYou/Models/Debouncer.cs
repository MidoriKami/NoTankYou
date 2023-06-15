using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoTankYou.Models;

public class Debouncer
{
    private readonly HashSet<uint> characterWaitList = new();

    private bool lastState;

    public void Update(uint objectId, bool state)
    {
        if (lastState && !state)
        {
            characterWaitList.Add(objectId);
            Task.Delay(500).ContinueWith(_ =>
            {
                characterWaitList.Remove(objectId);
            });
        }

        lastState = state;
    }

    public bool IsLockedOut(uint objectId) => characterWaitList.Contains(objectId);
}