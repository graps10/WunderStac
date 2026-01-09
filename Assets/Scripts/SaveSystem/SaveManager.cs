using UnityEngine;

namespace SaveSystem
{
    public static class SaveManager
    {
        // Game
        private const string Key_CurrentLevel = "Progress_CurrentLevel";
        
        // Settings
        private const string Key_SoundVol = "Settings_Sound_Vol";
        private const string Key_MusicVol = "Settings_Music_Vol";
        private const string Key_Vibration = "Settings_Vibration";

        public static int CurrentLevel
        {
            get => PlayerPrefs.GetInt(Key_CurrentLevel, 1);
            set
            {
                PlayerPrefs.SetInt(Key_CurrentLevel, value);
                PlayerPrefs.Save();
            }
        }
        
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(Key_MusicVol, 1f);
            set
            {
                PlayerPrefs.SetFloat(Key_MusicVol, value);
                PlayerPrefs.Save();
            }
        }

        public static float SoundVolume
        {
            get => PlayerPrefs.GetFloat(Key_SoundVol, 1f);
            set
            {
                PlayerPrefs.SetFloat(Key_SoundVol, value);
                PlayerPrefs.Save();
            }
        }

        public static bool IsVibrationOn
        {
            get => PlayerPrefs.GetInt(Key_Vibration, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(Key_Vibration, value ? 1 : 0);
                Save();
            }
        }

        private static void Save() => PlayerPrefs.Save();
    }
}