using DG.Tweening;
using TMPro;
using UnityEngine;

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
        
        [Header("Appearance Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.red;

        private int _displayedScore;
        
        public void UpdateScore(int newScore)
        {
            DOTween.To(() => _displayedScore, x => _displayedScore = x, newScore, Anim_Count_Duration)
                .OnUpdate(() => scoreText.text = _displayedScore.ToString());
            
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
    }
}