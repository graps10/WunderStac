using Game.Level;
using UI.Game;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private const int Bonus_Score_Per_Move = 100;
        
        public static GameManager Instance;

        [Header("References")]
        [SerializeField] private GameUIController uiController;
        
        [Header("Debug / Testing")]
        [SerializeField] private LevelProfile currentLevelProfile;
        
        public int CurrentScore { get; private set; }
        public int MovesLeft { get; private set; }
        public bool IsGameActive { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (currentLevelProfile != null)
                StartLevel(currentLevelProfile);
        }

        public void StartLevel(LevelProfile profile)
        {
            currentLevelProfile = profile;
            
            CurrentScore = 0;
            MovesLeft = profile.maxMoves;
            IsGameActive = true;
            
            Random.InitState(profile.randomSeed);
            if (uiController != null)
            {
                uiController.SetLevelText(profile.levelIndex);
                uiController.UpdateScore(0);
                uiController.UpdateMoves(MovesLeft);
            }
            
            Debug.Log($"Level {profile.levelIndex} Started!, Moves: {MovesLeft}");
        }
        
        public void AddScore(int amount)
        {
            if (!IsGameActive) return;
            
            CurrentScore += amount;
            if (uiController) uiController.UpdateScore(CurrentScore);
        }

        private void AddExtraScore(int amount)
        {
            CurrentScore += amount;
            if (uiController) uiController.UpdateScore(CurrentScore);
        }
        
        public void ConsumeMove()
        {
            if (!IsGameActive) return;

            MovesLeft--;
            if (uiController) uiController.UpdateMoves(MovesLeft);

            if (MovesLeft <= 0)
                CheckEndGameCondition();
        }
        
        public void WinLevel()
        {
            if (!IsGameActive) return;
            IsGameActive = false;
            
            int movesBonus = MovesLeft * Bonus_Score_Per_Move;
            AddExtraScore(movesBonus);
            
            int starCount = CalculateStars(CurrentScore);

            Debug.Log($"LEVEL WON! Score: {CurrentScore}, Stars: {starCount}");
            
            if (PopupManager.Instance != null)
                PopupManager.Instance.ShowWin(CurrentScore, starCount);
        }
        
        public void CheckEndGameCondition()
        {
            if (MovesLeft <= 0)
            {
                Debug.Log("No Moves Left! Checking for Win/Loss...");
                IsGameActive = false;
                
                PopupManager.Instance.ShowLose();
            }
        }
        
        private int CalculateStars(int score)
        {
            if (currentLevelProfile == null) return 1;

            if (score >= currentLevelProfile.scoreForThreeStars) return 3;
            if (score >= currentLevelProfile.scoreForTwoStars) return 2;
            return 1;
        }
        
        public LevelProfile GetCurrentProfile() => currentLevelProfile;
    }
}