using System.Collections;
using System.Collections.Generic;
using Game.Level;
using SaveSystem;
using UI.Controllers;
using UI.Game;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private const float Win_Seq_Delay_Start = 0.5f;
        private const float Win_Seq_Delay_End = 0.5f;
        private const int Infinity_Seed_Multiplier = 555;
        
        private const int Bonus_Score_Per_Move = 100;
        private const float Bonus_Count_Speed = 0.15f;
        
        public static GameManager Instance;

        [Header("References")]
        [SerializeField] private GameUIController uiController;
        
        [Header("Level Configuration")]
        [SerializeField] private List<LevelProfile> allLevels;
        
        public int CurrentScore { get; private set; }
        public int MovesLeft { get; private set; }
        public bool IsGameActive { get; private set; }
        public bool IsPaused { get; private set; }
        
        private LevelProfile _activeProfile;
        private int _currentLevelIndex;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _currentLevelIndex = SaveManager.CurrentLevel;
            PrepareLevel(_currentLevelIndex);
        }
        
        private void PrepareLevel(int levelIndex)
        {
            LevelProfile template;
            int seed;

            if (levelIndex <= allLevels.Count)
            {
                // Default Level Mode (Levels 1-10)
                template = allLevels[levelIndex - 1];
                seed = template.randomSeed;
            }
            else
            {
                // Infinity Mode (Level 11+)
                template = allLevels[^1]; 
                seed = levelIndex * Infinity_Seed_Multiplier; 
            }
            
            _activeProfile = Instantiate(template);
            _activeProfile.levelIndex = levelIndex;
            _activeProfile.randomSeed = seed;
            
            CurrentScore = 0;
            MovesLeft = _activeProfile.maxMoves;
            IsGameActive = false;
            
            Random.InitState(_activeProfile.randomSeed);
            
            if (uiController != null)
            {
                uiController.SetLevelText(levelIndex);
                uiController.UpdateScore(0);
                uiController.UpdateMoves(MovesLeft);
                uiController.InitScoreSlider(_activeProfile.scoreForThreeStars);
                uiController.ShowStartButton(true); 
            }
            
            Debug.Log($"Level {levelIndex} Prepared. Seed: {seed}");
        }

        public void StartLevel()
        {
            IsGameActive = true;
            IsPaused = false;
            Time.timeScale = 1f;
            
            GameEvents.OnGameStarted?.Invoke();
            
            if (uiController != null)
                uiController.ShowStartButton(false);
        }
        
        public void TogglePause()
        {
            if (!IsGameActive) return;

            IsPaused = !IsPaused;

            if (IsPaused)
            {
                Time.timeScale = 0f;
                PopupManager.Instance.ShowPause();
            }
            else
            {
                Time.timeScale = 1f;
                PopupManager.Instance.HidePause();
            }
        }
        
        public void AddScore(int amount)
        {
            if (!IsGameActive) return;
            
            CurrentScore += amount;
            if (uiController) uiController.UpdateScore(CurrentScore);
        }
        
        private int CalculateStars(int score)
        {
            if (_activeProfile == null) return 1;

            if (score >= _activeProfile.scoreForThreeStars) return 3;
            if (score >= _activeProfile.scoreForTwoStars) return 2;
            return 1;
        }
        
        public void ShowComboText(string text, Vector3 worldPos)
        {
            if (uiController)
                uiController.SpawnFloatingText(text, worldPos); 
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
            StartCoroutine(WinLevelRoutine());
        }

        private IEnumerator WinLevelRoutine()
        {
            IsGameActive = false;
            Debug.Log("Level Completed! Starting Bonus Sequence...");
            
            yield return new WaitForSeconds(Win_Seq_Delay_Start);
            
            while (MovesLeft > 0)
            {
                MovesLeft--;
                CurrentScore += Bonus_Score_Per_Move;
                
                if (uiController)
                {
                    uiController.UpdateMoves(MovesLeft);
                    uiController.UpdateScore(CurrentScore);
                }
                
                yield return new WaitForSeconds(Bonus_Count_Speed);
            }
            
            yield return new WaitForSeconds(Win_Seq_Delay_End);
            
            GameEvents.OnLevelWon?.Invoke();
            
            int starCount = CalculateStars(CurrentScore);
            SaveManager.CurrentLevel = _currentLevelIndex + 1;
            
            Debug.Log($"LEVEL WON! Score: {CurrentScore}, Stars: {starCount}");
            
            if (PopupManager.Instance != null)
                PopupManager.Instance.ShowWin(CurrentScore, starCount);
        }
        
        public void CheckEndGameCondition()
        {
            if (MovesLeft <= 0)
            {
                Debug.Log("No Moves Left! Checking Result...");
                IsGameActive = false;
                
                int stars = CalculateStars(CurrentScore);

                if (stars >= 1)
                {
                    Debug.Log("Moves out, but Score is enough! WIN.");
                    WinLevel(); 
                }
                else
                {
                    Debug.Log("Moves out and Score too low. LOSE.");
                    if (PopupManager.Instance != null)
                        PopupManager.Instance.ShowLose();
                        
                    GameEvents.OnLevelLost?.Invoke();
                }
            }
        }
        
        public LevelProfile GetCurrentProfile() => _activeProfile;
    }
}