using SaveSystem;
using UnityEngine;

namespace Utils
{
    public static class VibrationManager
    {
        public static void Vibrate()
        {
            if (!SaveManager.IsVibrationOn) return;
            
            Handheld.Vibrate();
            //Debug.Log("[Vibration] Buzz!");
        }
    }
}