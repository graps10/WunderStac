using UnityEngine;
using UnityEngine.UI;

namespace AudioSystem
{
    [RequireComponent(typeof(Button))]
    public class ClickButton : MonoBehaviour
    {
        [SerializeField] private SoundType clickSound = SoundType.ButtonClick;

        private Button _btn;

        private void Awake() => _btn = GetComponent<Button>();

        private void OnEnable() => _btn.onClick.AddListener(PlayClick);
        private void OnDisable() => _btn.onClick.RemoveListener(PlayClick);

        private void PlayClick()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound(clickSound);
        }
    }
}