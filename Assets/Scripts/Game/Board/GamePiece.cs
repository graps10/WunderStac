using DG.Tweening;
using UnityEngine;

namespace Game.Board
{
    public class GamePiece : MonoBehaviour
    {
        private const float Target_Destroy_Value = 0.3f;
        
        [SerializeField] private SpriteRenderer spriteRenderer;

        public int X;
        public int Y;
        public ItemType Type { get; private set; }

        public void Init(int x, int y, ItemType type, Sprite sprite)
        {
            X = x;
            Y = y;
            Type = type;
            spriteRenderer.sprite = sprite;
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public Tween MoveToPosition(Vector3 targetPosition, float time)
        {
            return transform.DOMove(targetPosition, time).SetEase(Ease.OutQuad);
        }

        public void DestroyPiece()
        {
            transform.DOScale(Vector3.zero, Target_Destroy_Value)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}