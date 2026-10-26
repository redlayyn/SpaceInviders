using TMPro;
using UnityEngine;
using SpaceInvaders.Core;

namespace SpaceInvaders.UI
{
    // счёт слева сверху
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private ScoreCounter _score;

        public void Init(ScoreCounter score)
        {
            _score = score;
            _score.Changed += Render;
            Render(_score.Value);
        }

        private void OnDestroy()
        {
            if (_score != null) _score.Changed -= Render;
        }

        private void Render(int value)
        {
            label.text = $"Score: {value}";
        }
    }
}
