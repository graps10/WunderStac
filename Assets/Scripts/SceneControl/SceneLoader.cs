using System.Collections;
using Core.Enums;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneControl
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private CanvasGroup faderCanvasGroup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            faderCanvasGroup.alpha = 0;
            faderCanvasGroup.blocksRaycasts = false;
        }

        public void LoadScene(UnityScenes scene) => StartCoroutine(LoadSceneRoutine(scene));

        private IEnumerator LoadSceneRoutine(UnityScenes scene)
        {
            faderCanvasGroup.blocksRaycasts = true;
            yield return faderCanvasGroup.DOFade(1f, fadeDuration).WaitForCompletion();
            
            yield return SceneManager.LoadSceneAsync(scene.ToString());
            
            yield return faderCanvasGroup.DOFade(0f, fadeDuration).WaitForCompletion();
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}