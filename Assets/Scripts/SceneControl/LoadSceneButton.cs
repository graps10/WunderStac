using Core.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace SceneControl
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private UnityScenes scene;
        
        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void OnEnable() => _button.onClick.AddListener(BackToMenu);
        private void OnDisable() => _button.onClick.RemoveListener(BackToMenu);

        private void BackToMenu() => SceneLoader.Instance.LoadScene(scene);
    }
}