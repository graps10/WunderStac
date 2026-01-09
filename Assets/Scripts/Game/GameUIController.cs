using DG.Tweening;
using TMPro;
using UI.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameUIController : MonoBehaviour
    {
        private const float Anim_Count_Duration = 0.5f;
        private const float Anim_Punch_Duration = 0.3f;
        private const float Anim_Punch_Strength = 0.2f;
        private const int Low_Moves_Threshold = 5;
        
        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI scoreText; 
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI levelText;
        
        [Header("Progress")]
        [SerializeField] private Slider scoreSlider;
        
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button pauseButton;
        
        [Header("Appearance Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;
        
        [Header("Floating Text")]
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private Transform floatingTextContainer;

        private int _displayedScore;
        
        private void OnEnable()
        {
            if (startButton) 
                startButton.onClick.AddListener(() => GameManager.Instance.StartLevel());
            
            if (pauseButton) 
                pauseButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
        }
        
        private void OnDisable()
        {
            if (startButton) startButton.onClick.RemoveAllListeners();
            if (pauseButton) pauseButton.onClick.RemoveAllListeners();
        }
        
        public void InitScoreSlider(int maxScore)
        {
            if (scoreSlider)
            {
                scoreSlider.maxValue = maxScore;
                scoreSlider.value = 0;
            }
        }
        
        public void ShowStartButton(bool show)
        {
            if (startButton) startButton.gameObject.SetActive(show);
            if (pauseButton) pauseButton.gameObject.SetActive(!show);
        }
        
        public void UpdateScore(int newScore)
        {
            DOTween.To(() => _displayedScore, x => _displayedScore = x, newScore, Anim_Count_Duration)
                .OnUpdate(() => 
                {
                    scoreText.text = _displayedScore.ToString();
                    if (scoreSlider) scoreSlider.value = _displayedScore;
                });
            
            scoreText.transform.DOKill(); 
            scoreText.transform.localScale = Vector3.one;
            scoreText.transform.DOPunchScale(Vector3.one * Anim_Punch_Strength, Anim_Punch_Duration);
        }

        public void UpdateMoves(int moves)
        {
            movesText.text = moves.ToString();
            
            movesText.color = moves <= Low_Moves_Threshold ? warningColor : normalColor;
            
            movesText.transform.DOKill();
            movesText.transform.localScale = Vector3.one;
            movesText.transform.DOPunchScale(Vector3.one * Anim_Punch_Strength, Anim_Punch_Duration);
        }
        
        public void SetLevelText(int level) => levelText.text = $"Level {level}";
        
        public void SpawnFloatingText(string text, Vector3 worldPos)
        {
            if (!floatingTextPrefab) return;

            var ft = Instantiate(floatingTextPrefab, floatingTextContainer);
            ft.Init(text, worldPos);
        }
    }
}