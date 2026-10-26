using UnityEngine;

namespace SpaceInvaders.Core
{
    // границы экрана в мировых координатах
    public readonly struct ScreenBounds
    {
        public float Left { get; }
        public float Right { get; }
        public float Bottom { get; }
        public float Top { get; }

        public ScreenBounds(Camera camera)
        {
            float halfHeight = camera.orthographicSize;
            float halfWidth = halfHeight * camera.aspect;
            Vector3 center = camera.transform.position;

            Left = center.x - halfWidth;
            Right = center.x + halfWidth;
            Bottom = center.y - halfHeight;
            Top = center.y + halfHeight;
        }

        public bool IsOutsideVertically(float y)
        {
            return y > Top || y < Bottom;
        }
    }
}
