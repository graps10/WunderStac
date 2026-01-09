using AudioSystem;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Controllers
{
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("Controls")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button languageButton;
        
        [Header("Toggle Settings")]
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private ToggleImageChanger toggleImageChanger;
        private void OnEnable()
        {
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            
            bool isVibroOn = SaveManager.IsVibrationOn;
            vibrationToggle.isOn = isVibroOn;
            if (toggleImageChanger) toggleImageChanger.UpdateImage(isVibroOn);
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
        }

        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
            vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
        }
        
        private void Start() => InitializeValues();

        private void InitializeValues()
        {
            musicSlider.value = SaveManager.MusicVolume;
            sfxSlider.value = SaveManager.SoundVolume;
            vibrationToggle.isOn = SaveManager.IsVibrationOn;
            toggleImageChanger.UpdateImage(SaveManager.IsVibrationOn);
        }

        private void OnMusicChanged(float value) => AudioManager.Instance.SetMusicVolume(value);

        private void OnSFXChanged(float value) => AudioManager.Instance.SetSFXVolume(value);

        private void OnVibrationChanged(bool isOn)
        {
            SaveManager.IsVibrationOn = isOn;
            
            toggleImageChanger.UpdateImage(isOn);
            if(isOn) VibrationManager.Vibrate();
        }
    }
}