using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceInvaders.Player;

namespace SpaceInvaders.UI
{
    // иконки хп, 1 хп = 1 иконка
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image iconPrefab;
        [SerializeField] private Transform container;

        private readonly List<Image> _icons = new List<Image>();
        private PlayerHealth _health;

        public void Init(PlayerHealth health)
        {
            _health = health;

            for (int i = 0; i < _health.Max; i++)
                _icons.Add(Instantiate(iconPrefab, container));

            _health.Changed += Render;
            Render(_health.Current);
        }

        private void OnDestroy()
        {
            if (_health != null) _health.Changed -= Render;
        }

        private void Render(int current)
        {
            for (int i = 0; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(i < current);
        }
    }
}
