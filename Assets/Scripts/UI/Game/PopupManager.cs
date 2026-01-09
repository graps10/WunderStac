using Core.Enums;
using DG.Tweening;
using Game;
using SceneControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class PopupManager : MonoBehaviour
    {
        private const float Anim_Popup_Duration = 0.5f;
        private const float Anim_Star_Punch_Scale = 0.3f;
        private const float Anim_Star_Punch_Duration = 0.5f;
        private const float Anim_Star_Delay = 0.2f;
        private const float Anim_Pause_Duration = 0.5f;
        
        private const int Min_Stars = 1;
        private const int Max_Stars = 3;
        
        public static PopupManager Instance;

        [Header("Popups")]
        [SerializeField] private GameObject winPopup;
        [SerializeField] private GameObject losePopup;
        [SerializeField] private GameObject pausePopup;

        [Header("Popup Elements")]
        [SerializeField] private Image winTitleImage;
        [SerializeField] private Image loseTitleImage;
        [SerializeField] private Image winStarsImage;
        [SerializeField] private TextMeshProUGUI winScoreText;

        [Header("Sprite Database")]
        //  [0]=Bad, [1]=Good, [2]=Great, [3]=Perfect
        [SerializeField] private Sprite[] titleSprites;
        
        //  [0]=1 Star, [1]=2 Stars, [2]=3 Stars
        [SerializeField] private Sprite[] starSprites;
        
        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button resumeButton;

        private void Awake()
        {
            Instance = this;
            winPopup.SetActive(false);
            losePopup.SetActive(false);
            if (pausePopup) pausePopup.SetActive(false);
        }

        private void OnEnable()
        {
            if (nextButton) nextButton.onClick.AddListener(OnNextClicked);
            if (retryButton) retryButton.onClick.AddListener(OnRetryClicked);

            if (resumeButton)
            {
                resumeButton.onClick.AddListener(GameManager.Instance.TogglePause);
                resumeButton.onClick.AddListener(HidePause);
            }
                
        }

        private void OnDisable()
        {
            if (nextButton) nextButton.onClick.RemoveListener(OnNextClicked);
            if (retryButton) retryButton.onClick.RemoveListener(OnRetryClicked);
            
            if (resumeButton)
            {
                resumeButton.onClick.RemoveListener(GameManager.Instance.TogglePause);
                resumeButton.onClick.RemoveListener(HidePause);
            }
        }
        
        public void ShowWin(int score, int starCount)
        {
            int clampedStars = Mathf.Clamp(starCount, Min_Stars, Max_Stars);

            winPopup.SetActive(true);
            AnimatePopup(winPopup.transform);

            if (winScoreText) winScoreText.text = $"{score}";
            
            if (winStarsImage && starSprites.Length >= clampedStars)
            {
                winStarsImage.sprite = starSprites[clampedStars - 1];
                winStarsImage.transform.DOPunchScale(Vector3.one * Anim_Star_Punch_Scale, Anim_Star_Punch_Duration)
                    .SetDelay(Anim_Star_Delay);
            }
            
            if (winTitleImage && titleSprites.Length > clampedStars)
            {
                winTitleImage.sprite = titleSprites[clampedStars];
                winTitleImage.SetNativeSize();
            }
        }
        
        public void ShowLose()
        {
            losePopup.SetActive(true);
            AnimatePopup(losePopup.transform);
            
            if (loseTitleImage && titleSprites.Length > 0)
                loseTitleImage.sprite = titleSprites[0];
        }
        
        public void ShowPause()
        {
            pausePopup.SetActive(true);
            pausePopup.transform.localScale = Vector3.zero;
            pausePopup.transform.DOScale(1f, Anim_Pause_Duration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        public void HidePause() => pausePopup.SetActive(false);

        private void AnimatePopup(Transform popupContent)
        {
            popupContent.localScale = Vector3.zero;
            popupContent.DOScale(1f, Anim_Popup_Duration).SetEase(Ease.OutBack);
        }

        private void OnNextClicked() => SceneLoader.Instance.LoadScene(UnityScenes.Game);
        private void OnRetryClicked() => SceneLoader.Instance.LoadScene(UnityScenes.Game);

        #region Test
        
        [ContextMenu("Test Win: 1 Star")] public void TestWin1() => ShowWin(1000, 1);
        [ContextMenu("Test Win: 2 Stars")] public void TestWin2() => ShowWin(2500, 2);
        [ContextMenu("Test Win: 3 Stars")] public void TestWin3() => ShowWin(4500, 3);
        [ContextMenu("Test Lose")] public void TestLose() => ShowLose();
        
        #endregion
        
    }
}