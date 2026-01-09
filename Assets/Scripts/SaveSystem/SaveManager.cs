using UnityEngine;

namespace SaveSystem
{
    public static class SaveManager
    {
        // Progression
        private const string Key_CurrentLevel = "Progress_CurrentLevel";
        
        // Stats
        private const string Key_BestScore = "Stats_BestScore";
        private const string Key_HighestCombo = "Stats_HighestCombo";
        private const string Key_GamesPlayed = "Stats_GamesPlayed";
        private const string Key_WordsCompleted = "Stats_WordsCompleted";
        
        // Settings
        private const string Key_SoundVol = "Settings_Sound_Vol";
        private const string Key_MusicVol = "Settings_Music_Vol";
        private const string Key_Vibration = "Settings_Vibration";

        public static int CurrentLevel
        {
            get => PlayerPrefs.GetInt(Key_CurrentLevel, 1);
            set { PlayerPrefs.SetInt(Key_CurrentLevel, value); Save(); }
        }
        
        public static int BestScore
        {
            get => PlayerPrefs.GetInt(Key_BestScore, 0);
            set 
            {
                if (value > BestScore) 
                {
                    PlayerPrefs.SetInt(Key_BestScore, value);
                    Save();
                }
            }
        }

        public static int HighestCombo
        {
            get => PlayerPrefs.GetInt(Key_HighestCombo, 0);
            set 
            {
                if (value > HighestCombo)
                {
                    PlayerPrefs.SetInt(Key_HighestCombo, value);
                    Save();
                }
            }
        }

        public static int GamesPlayed
        {
            get => PlayerPrefs.GetInt(Key_GamesPlayed, 0);
            set { PlayerPrefs.SetInt(Key_GamesPlayed, value); Save(); }
        }

        public static int WordsCompleted
        {
            get => PlayerPrefs.GetInt(Key_WordsCompleted, 0);
            set { PlayerPrefs.SetInt(Key_WordsCompleted, value); Save(); }
        }
        
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(Key_MusicVol, 1f);
            set
            {
                PlayerPrefs.SetFloat(Key_MusicVol, value);
                Save();
            }
        }

        public static float SoundVolume
        {
            get => PlayerPrefs.GetFloat(Key_SoundVol, 1f);
            set
            {
                PlayerPrefs.SetFloat(Key_SoundVol, value);
                Save();
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