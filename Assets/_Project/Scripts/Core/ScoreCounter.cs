using System;

namespace SpaceInvaders.Core
{
    // счёт, событие только при изменении
    public class ScoreCounter
    {
        public int Value { get; private set; }

        public event Action<int> Changed;

        public void Add(int points)
        {
            if (points == 0) return;

            Value += points;
            Changed?.Invoke(Value);
        }

        public void Reset()
        {
            if (Value == 0) return;

            Value = 0;
            Changed?.Invoke(Value);
        }
    }
}
