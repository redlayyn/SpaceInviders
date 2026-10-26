using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SpaceInvaders.Player
{
    // чтение ввода
    public class InputReader : MonoBehaviour
    {
        public float Horizontal { get; private set; }

        public event Action FirePressed;
        public event Action RestartPressed;

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            float h = 0f;
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) h -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) h += 1f;
            Horizontal = h;

            if (keyboard.spaceKey.wasPressedThisFrame) FirePressed?.Invoke();
            if (keyboard.rKey.wasPressedThisFrame) RestartPressed?.Invoke();
#else
            Horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space)) FirePressed?.Invoke();
            if (Input.GetKeyDown(KeyCode.R)) RestartPressed?.Invoke();
#endif
        }
    }
}
