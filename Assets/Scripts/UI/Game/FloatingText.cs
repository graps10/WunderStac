using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    public class FloatingText : MonoBehaviour
    {
        private const float Anim_Scale_Duration = 0.3f;
        private const float Anim_Fade_Duration = 0.3f;
        private const float Anim_Fade_Delay_Offset = 0.3f;
        
        [SerializeField] private TextMeshProUGUI tmpText;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private float moveDistance = 100f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;

        public void Init(string text, Vector3 worldPos)
        {
            tmpText.text = text;
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
            
            Sequence seq = DOTween.Sequence();
            transform.localScale = Vector3.zero;
            seq.Append(transform.DOScale(1f, Anim_Scale_Duration).SetEase(Ease.OutBack));
            seq.Join(transform.DOLocalMoveY(transform.localPosition.y + moveDistance, moveDuration).SetEase(moveEase));
            seq.Join(tmpText.DOFade(0f, Anim_Fade_Duration).SetDelay(moveDuration - Anim_Fade_Delay_Offset));
            seq.OnComplete(() => Destroy(gameObject));
        }
    }
}