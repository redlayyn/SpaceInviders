using UnityEngine;
using SpaceInvaders.Enemies;
using SpaceInvaders.Player;
using SpaceInvaders.UI;

namespace SpaceInvaders.Core
{
    // точка входа, здесь всё собирается
    [DefaultExecutionOrder(-100)]
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Сцена")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private InputReader input;
        [SerializeField] private GameController game;

        [Header("Игрок")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private PlayerHealth playerHealth;

        [Header("Мобы")]
        [SerializeField] private EnemyGrid enemyGrid;
        [SerializeField] private EnemyShooter enemyShooter;

        [Header("UI")]
        [SerializeField] private ScoreView scoreView;
        [SerializeField] private HealthView healthView;
        [SerializeField] private EndScreenView endScreen;

        private ScoreCounter _score;

        private void Start()
        {
            Time.timeScale = 1f;

            var bounds = new ScreenBounds(mainCamera);
            _score = new ScoreCounter();

            playerMovement.Init(input, bounds);
            playerShooter.Init(input);

            float loseLineY = playerMovement.transform.position.y;
            enemyGrid.Init(bounds, loseLineY);
            enemyGrid.EnemyKilled += enemy => _score.Add(enemy.ScoreReward);
            enemyShooter.Init(enemyGrid);

            game.Init(playerHealth, enemyGrid, input);

            scoreView.Init(_score);
            healthView.Init(playerHealth);
            endScreen.Init(game, _score);
        }
    }
}
