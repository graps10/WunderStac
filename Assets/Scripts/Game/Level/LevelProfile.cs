using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "Level_01", menuName = "Game/Level Profile")]
    public class LevelProfile : ScriptableObject
    {
        [Header("General")]
        public int levelIndex = 1;
        public int randomSeed = 12345;

        [Header("Objective")]
        public int maxMoves = 30;

        [Header("Scoring")]
        public int scoreForOneStar = 1000;
        public int scoreForTwoStars = 2500;
        public int scoreForThreeStars = 4500;
        
        [Header("Economy")]
        public int scorePerPiece = 10;
        public int scorePerLetter = 50;
    }
}