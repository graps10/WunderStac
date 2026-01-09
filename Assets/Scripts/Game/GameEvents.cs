using System;

namespace Game
{
    public static class GameEvents
    {
        // Gameplay
        public static Action OnGameStarted;
        public static Action OnLevelWon;
        public static Action OnLevelLost;
        
        // Stats
        public static Action<int> OnComboUpdated; 
        public static Action OnWordCompleted;
        
        // UI
        public static Action<int> OnScoreChanged;
        public static Action<int> OnMovesChanged;
    }
}