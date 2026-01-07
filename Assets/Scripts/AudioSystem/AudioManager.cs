using SaveSystem;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        private const string Mixer_Music = "MusicVol";
        private const string Mixer_SFX = "SFXVol";

        private const float Min_Volume_Threshold = 0.001f;
        private const float Mute_Db = -80f;
        private const float Db_Multiplier = 20f;

        public static AudioManager Instance;

        [Header("Settings")] [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioLibrary audioLibrary;

        [Header("Sources")] [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            SetMusicVolume(SaveManager.MusicVolume);
            SetSFXVolume(SaveManager.SoundVolume);

            PlayMusic(SoundType.BackgroundMusic);
        }

        public void PlaySound(SoundType type)
        {
            if (SaveManager.SoundVolume <= Min_Volume_Threshold) return;

            AudioClip clip = audioLibrary.GetClip(type, out float vol);
            if (clip != null)
                sfxSource.PlayOneShot(clip, vol);
        }

        public void PlayMusic(SoundType type)
        {
            AudioClip clip = audioLibrary.GetClip(type, out float vol);
            if (clip != null && musicSource.clip != clip)
            {
                musicSource.clip = clip;
                musicSource.volume = vol;
                musicSource.Play();
            }
        }

        public void SetMusicVolume(float sliderValue)
        {
            SaveManager.MusicVolume = sliderValue;
            SetVolumeToMixer(Mixer_Music, sliderValue);
        }

        public void SetSFXVolume(float sliderValue)
        {
            SaveManager.SoundVolume = sliderValue;
            SetVolumeToMixer(Mixer_SFX, sliderValue);
        }
        
        private void SetVolumeToMixer(string parameterName, float sliderValue)
        {
            float db;
            if (sliderValue <= Min_Volume_Threshold)
                db = Mute_Db;
            else
                db = Mathf.Log10(sliderValue) * Db_Multiplier;

            mainMixer.SetFloat(parameterName, db);
        }
    }
}