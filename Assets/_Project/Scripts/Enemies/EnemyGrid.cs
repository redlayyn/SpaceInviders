using System;
using System.Collections.Generic;
using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.Enemies
{
    // сетка мобов и спуск вниз
    public class EnemyGrid : MonoBehaviour
    {
        [Header("Сетка")]
        [SerializeField] private Enemy[] rowPrefabs;
        [SerializeField, Min(1)] private int columns = 8;
        [SerializeField, Min(1)] private int rows = 4;
        [SerializeField] private Vector2 spacing = new Vector2(1.2f, 0.9f);
        [SerializeField] private float topMargin = 1.5f;

        [Header("Спуск")]
        [SerializeField, Min(0.1f)] private float stepInterval = 1.5f;
        [SerializeField, Min(0.01f)] private float stepDistance = 0.4f;
        [SerializeField, Min(0f)] private float horizontalStepDistance = 0.4f;

        private readonly List<Enemy> _enemies = new List<Enemy>();
        private float _loseLineY;
        private float _nextStepTime;
        private int _stepIndex;

        public IReadOnlyList<Enemy> Enemies => _enemies;

        public event Action<Enemy> EnemyKilled;
        public event Action Cleared;
        public event Action Reached;

        public void Init(ScreenBounds bounds, float loseLineY)
        {
            _loseLineY = loseLineY;
            _nextStepTime = Time.time + stepInterval;

            Spawn(bounds);
        }

        private void Spawn(ScreenBounds bounds)
        {
            if (rowPrefabs == null || rowPrefabs.Length == 0)
                throw new InvalidOperationException("EnemyGrid: не назначены префабы мобов (Row Prefabs)");

            float gridWidth = (columns - 1) * spacing.x;
            float startX = (bounds.Left + bounds.Right) * 0.5f - gridWidth * 0.5f;
            float startY = bounds.Top - topMargin;

            for (int row = 0; row < rows; row++)
            {
                Enemy prefab = rowPrefabs[Mathf.Min(row, rowPrefabs.Length - 1)];
                if (prefab == null)
                    throw new InvalidOperationException("EnemyGrid: в Row Prefabs пустой элемент");

                for (int col = 0; col < columns; col++)
                {
                    Vector3 position = new Vector3(startX + col * spacing.x, startY - row * spacing.y, 0f);
                    Enemy enemy = Instantiate(prefab, position, Quaternion.identity, transform);
                    enemy.Killed += OnEnemyKilled;
                    _enemies.Add(enemy);
                }
            }
        }

        private void Update()
        {
            if (_enemies.Count == 0) return;

            if (Time.time >= _nextStepTime)
            {
                _nextStepTime += stepInterval;
                MakeStep();
            }
        }

        // цикл шагов: вниз, вправо, вниз, влево
        private void MakeStep()
        {
            switch (_stepIndex % 4)
            {
                case 1:
                    transform.position += Vector3.right * horizontalStepDistance;
                    break;
                case 3:
                    transform.position += Vector3.left * horizontalStepDistance;
                    break;
                default:
                    transform.position += Vector3.down * stepDistance;
                    break;
            }

            _stepIndex++;

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.transform.position.y <= _loseLineY)
                {
                    Reached?.Invoke();
                    return;
                }
            }
        }

        private void OnEnemyKilled(Enemy enemy)
        {
            enemy.Killed -= OnEnemyKilled;
            _enemies.Remove(enemy);
            EnemyKilled?.Invoke(enemy);

            if (_enemies.Count == 0)
                Cleared?.Invoke();
        }
    }
}