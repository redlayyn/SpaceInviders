using System;
using UnityEngine;
using SpaceInvaders.Core;
using SpaceInvaders.Projectiles;

namespace SpaceInvaders.Enemies
{
    // один моб
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField, Min(0)] private int scoreReward = 10;
        [SerializeField] private Projectile bulletPrefab;

        private Health _health;

        public int ScoreReward => scoreReward;

        public event Action<Enemy> Killed;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (_health != null) _health.Died -= OnDied;
        }

        public void Shoot()
        {
            Projectile bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.Launch(Vector2.down);
        }

        private void OnDied(Health _)
        {
            Killed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
