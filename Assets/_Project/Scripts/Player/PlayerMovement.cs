using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.Player
{
    // движение игрока по горизонтали
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 8f;

        private InputReader _input;
        private float _minX;
        private float _maxX;

        public void Init(InputReader input, ScreenBounds bounds)
        {
            _input = input;

            float halfWidth = GetComponent<Collider2D>().bounds.extents.x;
            _minX = bounds.Left + halfWidth;
            _maxX = bounds.Right - halfWidth;
        }

        private void Update()
        {
            if (_input == null) return;

            Vector3 position = transform.position;
            position.x += _input.Horizontal * speed * Time.deltaTime;
            position.x = Mathf.Clamp(position.x, _minX, _maxX);
            transform.position = position;
        }
    }
}
