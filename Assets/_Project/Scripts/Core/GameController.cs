using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpaceInvaders.Enemies;
using SpaceInvaders.Player;

namespace SpaceInvaders.Core
{
    // победа, поражение, рестарт
    public class GameController : MonoBehaviour
    {
        private PlayerHealth _playerHealth;
        private EnemyGrid _enemyGrid;
        private InputReader _input;

        public GameState State { get; private set; } = GameState.Playing;

        public event Action<GameState> StateChanged;

        public void Init(PlayerHealth playerHealth, EnemyGrid enemyGrid, InputReader input)
        {
            _playerHealth = playerHealth;
            _enemyGrid = enemyGrid;
            _input = input;

            _playerHealth.Died += OnPlayerDied;
            _enemyGrid.Reached += OnEnemiesReachedPlayer;
            _enemyGrid.Cleared += OnEnemiesCleared;
            _input.RestartPressed += Restart;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null) _playerHealth.Died -= OnPlayerDied;
            if (_enemyGrid != null)
            {
                _enemyGrid.Reached -= OnEnemiesReachedPlayer;
                _enemyGrid.Cleared -= OnEnemiesCleared;
            }
            if (_input != null) _input.RestartPressed -= Restart;
        }

        private void OnPlayerDied() => SetState(GameState.Lost);
        private void OnEnemiesReachedPlayer() => SetState(GameState.Lost);
        private void OnEnemiesCleared() => SetState(GameState.Won);

        private void SetState(GameState newState)
        {
            if (State != GameState.Playing) return;

            State = newState;
            Time.timeScale = 0f;
            StateChanged?.Invoke(State);
        }

        private void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
