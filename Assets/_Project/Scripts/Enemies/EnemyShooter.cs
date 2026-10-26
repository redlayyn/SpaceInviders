using UnityEngine;

namespace SpaceInvaders.Enemies
{
    // случайный моб стреляет раз в min-max секунд
    public class EnemyShooter : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float minInterval = 0.7f;
        [SerializeField, Min(0.1f)] private float maxInterval = 2f;

        private EnemyGrid _grid;
        private float _nextShotTime;

        public void Init(EnemyGrid grid)
        {
            _grid = grid;
            ScheduleNext();
        }

        private void OnValidate()
        {
            if (maxInterval < minInterval) maxInterval = minInterval;
        }

        private void Update()
        {
            if (_grid == null || _grid.Enemies.Count == 0) return;
            if (Time.time < _nextShotTime) return;

            int index = Random.Range(0, _grid.Enemies.Count);
            _grid.Enemies[index].Shoot();
            ScheduleNext();
        }

        private void ScheduleNext()
        {
            _nextShotTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }
}
