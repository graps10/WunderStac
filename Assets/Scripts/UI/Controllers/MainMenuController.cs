using System.Collections.Generic;
using Core.Enums;
using DG.Tweening;
using SceneControl;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        private const float Target_Scale = 1f;
        private const float Title_Anim_Duration = 0.6f;
        private const float Button_Anim_Start_Delay = 0.2f;
        
        [Header("UI References")]
        [SerializeField] private Transform gameTitle;
        
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button statsButton;
        [SerializeField] private Button settingsButton;

        [Header("Animation Settings")]
        [SerializeField] private float animDuration = 0.4f;
        [SerializeField] private float staggerDelay = 0.1f;
        [SerializeField] private Ease easeType = Ease.OutBack;

        private void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            statsButton.onClick.AddListener(OnStatsClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            
            AnimateEntrance();
        }

        private void OnDisable()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            statsButton.onClick.RemoveListener(OnStatsClicked);
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            
            DOTween.Kill(transform); 
        }

        private void AnimateEntrance()
        {
            gameTitle.localScale = Vector3.zero;
            playButton.transform.localScale = Vector3.zero;
            statsButton.transform.localScale = Vector3.zero;
            settingsButton.transform.localScale = Vector3.zero;
            
            var buttons = new List<Transform> 
            { 
                playButton.transform, 
                statsButton.transform, 
                settingsButton.transform 
            };
            
            Sequence seq = DOTween.Sequence();
            
            seq.Append(gameTitle.DOScale(Target_Scale, Title_Anim_Duration).SetEase(Ease.OutElastic)); 
            
            float startTime = Button_Anim_Start_Delay; 

            foreach (var btn in buttons)
            {
                seq.Insert(startTime, btn.DOScale(Target_Scale, animDuration).SetEase(easeType));
                startTime += staggerDelay;
            }
        }

        private void OnPlayClicked() => SceneLoader.Instance.LoadScene(UnityScenes.Game);
        private void OnStatsClicked() => SceneLoader.Instance.LoadScene(UnityScenes.Stats);
        private void OnSettingsClicked() => SceneLoader.Instance.LoadScene(UnityScenes.Settings);
    }
}