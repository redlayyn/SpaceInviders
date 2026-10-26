using TMPro;
using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.UI
{
    // плашка победы и поражения
    public class EndScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text details;

        private GameController _game;
        private ScoreCounter _score;

        public void Init(GameController game, ScoreCounter score)
        {
            _game = game;
            _score = score;
            panel.SetActive(false);
            _game.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (_game != null) _game.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            bool won = state == GameState.Won;

            title.text = won ? "Победа! Поздравляем!" : "Поражение";
            details.text = $"Счёт: {_score.Value}\nНажмите R, чтобы начать заново";
            panel.SetActive(true);
        }
    }
}
