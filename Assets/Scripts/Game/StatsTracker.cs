using SaveSystem;
using UnityEngine;

namespace Game
{
    public class StatsTracker : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnComboUpdated += HandleCombo;
            GameEvents.OnWordCompleted += HandleWordCompleted;
            GameEvents.OnLevelWon += HandleLevelWon;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnComboUpdated -= HandleCombo;
            GameEvents.OnWordCompleted -= HandleWordCompleted;
            GameEvents.OnLevelWon -= HandleLevelWon;
        }

        private void HandleGameStarted() => SaveManager.GamesPlayed++;
        private void HandleCombo(int comboCount) => SaveManager.HighestCombo = comboCount;
        private void HandleWordCompleted() => SaveManager.WordsCompleted++;

        private void HandleLevelWon()
        {
            if (GameManager.Instance != null)
                SaveManager.BestScore = GameManager.Instance.CurrentScore;
        }
    }
}