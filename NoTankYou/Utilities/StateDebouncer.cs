using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoTankYou.Utilities
{
    internal class StateDebouncer<T> where T : notnull
    {
        private readonly Queue<T> StateList = new();
        private readonly int MaxSize = 10;

        public StateDebouncer(int size)
        {
            MaxSize = size;
        }

        public void AddState(T state)
        {
            if(StateList.Count < MaxSize)
            {
                StateList.Enqueue(state);
            }
            else
            {
                StateList.Dequeue();
                StateList.Enqueue(state);
            }
        }

        public T EvaluateState()
        {
            if(StateList.Count == 0) return default!;

            Dictionary<T, int> stateCounter = new();

            foreach(T state in StateList)
            {
                if(stateCounter.ContainsKey(state))
                {
                    stateCounter[state]++;
                }
                else
                {
                    stateCounter.Add(state, 1);
                }
            }

            // Get the key with the largest value
            T largestThing = stateCounter.Aggregate((x,y) => x.Value > y.Value ? x : y).Key;

            return largestThing;
        }
    }
}
