using UnityEngine;
using SpaceInvaders.Projectiles;

namespace SpaceInvaders.Player
{
    // выстрел игрока вверх
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Projectile bulletPrefab;
        [SerializeField] private Transform muzzle;
        [SerializeField, Min(0f)] private float cooldown = 0.35f;

        private InputReader _input;
        private float _nextShotTime;

        public void Init(InputReader input)
        {
            _input = input;
            _input.FirePressed += TryShoot;
        }

        private void OnDestroy()
        {
            if (_input != null) _input.FirePressed -= TryShoot;
        }

        private void TryShoot()
        {
            if (Time.timeScale == 0f || Time.time < _nextShotTime) return;

            _nextShotTime = Time.time + cooldown;
            Projectile bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
            bullet.Launch(Vector2.up);
        }
    }
}
