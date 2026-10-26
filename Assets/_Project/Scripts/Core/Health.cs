using System;
using UnityEngine;

namespace SpaceInvaders.Core
{
    // здоровье для игрока и мобов
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int maxHealth = 1;

        private int _current;

        public int Current => _current;
        public int Max => maxHealth;

        public event Action<int> Changed;
        public event Action<Health> Died;

        private void Awake()
        {
            _current = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (_current <= 0) return;

            _current = Mathf.Max(0, _current - amount);
            Changed?.Invoke(_current);

            if (_current == 0)
                Died?.Invoke(this);
        }
    }
}
