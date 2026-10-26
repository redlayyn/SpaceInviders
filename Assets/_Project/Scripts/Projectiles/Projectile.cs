using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.Projectiles
{
    // пуля игрока и мобов
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 12f;
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField, Min(0.1f)] private float lifetime = 5f;

        private Vector2 _direction;

        public void Launch(Vector2 direction)
        {
            _direction = direction.normalized;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(_direction * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

            if (other.TryGetComponent(out IDamageable target))
                target.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
