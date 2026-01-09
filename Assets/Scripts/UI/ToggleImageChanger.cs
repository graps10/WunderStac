using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleImageChanger : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Image targetImage;

        [Header("Sprites")] [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private Toggle _toggle;

        private void Awake() => _toggle = GetComponent<Toggle>();

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(UpdateImage);
            UpdateImage(_toggle.isOn);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(UpdateImage);
        }

        public void UpdateImage(bool isOn)
        {
            if (targetImage != null)
                targetImage.sprite = isOn ? onSprite : offSprite;
        }
    }
}