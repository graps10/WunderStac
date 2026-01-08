using System.Collections.Generic;
using UI.Game;
using UnityEngine;

namespace Game.Board
{
    [System.Serializable]
    public class PieceProvider
    {
        [Header("Config")]
        [Range(0f, 1f)] public float baseLetterChance = 0.1f;
        [SerializeField] private int guaranteedSpawnThreshold = 15;
        
        private int _spawnsSinceLastLetter;
        
        public ItemType GetNextType()
        {
            float currentChance = baseLetterChance;
            
            if (_spawnsSinceLastLetter >= guaranteedSpawnThreshold) 
                currentChance = 1.0f;
            
            if (Random.value < currentChance)
            {
                if (WordManager.Instance != null)
                {
                    List<ItemType> neededLetters = WordManager.Instance.GetNeededLetterTypes();
                    
                    if (neededLetters.Count > 0)
                    {
                        _spawnsSinceLastLetter = 0;
                        int randomIndex = Random.Range(0, neededLetters.Count);
                        return neededLetters[randomIndex];
                    }
                }
            }
            
            _spawnsSinceLastLetter++;
            return (ItemType)Random.Range(1, 7);
        }

        public ItemType GetRandomNormalType() => (ItemType)Random.Range(1, 7);
    }
}