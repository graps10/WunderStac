using UnityEngine;
using Utils;

namespace Game
{
    public class GameVibrationController : MonoBehaviour
    {
        private const int Vibration_Combo_Count = 4;
        
        private void OnEnable()
        {
            GameEvents.OnLevelWon += VibrateSuccess;
            GameEvents.OnLevelLost += VibrateFailure;
            GameEvents.OnWordCompleted += VibrateSuccess;

            GameEvents.OnComboUpdated += HandleCombo;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelWon -= VibrateSuccess;
            GameEvents.OnLevelLost -= VibrateFailure;
            GameEvents.OnWordCompleted -= VibrateSuccess;

            GameEvents.OnComboUpdated -= HandleCombo;
        }

        private void VibrateSuccess() => VibrationManager.Vibrate();
        private void VibrateFailure() => VibrationManager.Vibrate();

        private void HandleCombo(int comboCount)
        {
            if (comboCount >= Vibration_Combo_Count)
                VibrationManager.Vibrate();
        }
    }
}