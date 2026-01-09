using SaveSystem;
using TMPro;
using UnityEngine;

namespace UI.Controllers
{
    public class StatsMenuController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI highestComboText;
        [SerializeField] private TextMeshProUGUI gamesPlayedText;
        [SerializeField] private TextMeshProUGUI wordsCompletedText;

        private void Start() => UpdateStatsUI();

        private void UpdateStatsUI()
        {
            // Level
            if (currentLevelText) 
                currentLevelText.text = $"Level {SaveManager.CurrentLevel}";

            // Best Score
            if (bestScoreText)
                bestScoreText.text = SaveManager.BestScore.ToString();

            // Combo
            if (highestComboText)
            {
                int comboCount = SaveManager.HighestCombo;
                highestComboText.text = comboCount > 1 ? $"{comboCount}x" : "-";
            }
            
            // Games Played
            if (gamesPlayedText)
                gamesPlayedText.text = SaveManager.GamesPlayed.ToString();

            // Words
            if (wordsCompletedText)
                wordsCompletedText.text = SaveManager.WordsCompleted.ToString();
        }
    }
}