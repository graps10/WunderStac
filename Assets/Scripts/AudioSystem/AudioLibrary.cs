using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Audio Library")]
    public class AudioLibrary : ScriptableObject
    {
        private const float Max_Sound_Volume = 1.0f;
        
        [System.Serializable]
        public class SoundData
        {
            public SoundType type;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = Max_Sound_Volume;
        }

        public List<SoundData> Sounds;

        public AudioClip GetClip(SoundType type, out float volume)
        {
            foreach (var sound in Sounds)
            {
                if (sound.type == type)
                {
                    volume = sound.volume;
                    return sound.clip;
                }
            }
            
            //Debug.LogWarning($"Sound not found: {type}");
            volume = Max_Sound_Volume;
            return null;
        }
    }
}