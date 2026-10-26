using System;
using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.Player
{
    // события здоровья игрока
    [RequireComponent(typeof(Health))]
    public class PlayerHealth : MonoBehaviour
    {
        private Health _health;

        public int Current => _health.Current;
        public int Max => _health.Max;

        public event Action<int> Changed;
        public event Action Died;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.Changed += hp => Changed?.Invoke(hp);
            _health.Died += _ => Died?.Invoke();
        }
    }
}
