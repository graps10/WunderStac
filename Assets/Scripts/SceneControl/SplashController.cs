using Core.Enums;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SceneControl
{
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private float fakeLoadingTime = 2.0f;
        
        private void Start() => StartLoading();

        private void StartLoading()
        {
            loadingSlider.value = 0;
            
            loadingSlider.DOValue(1f, fakeLoadingTime)
                .SetEase(Ease.InOutQuad)
                .OnComplete(OnLoadingFinished);
        }

        private void OnLoadingFinished() => SceneLoader.Instance.LoadScene(UnityScenes.MainMenu);
    }
}