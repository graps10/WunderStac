using System.Collections.Generic;
using DG.Tweening;
using Game.Board;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class WordManager : MonoBehaviour
    {
        private const float Anim_Target_Scale = 0.5f;
        private const float Anim_Punch_Strength = 0.2f;
        private const float Anim_Punch_Duration = 0.3f;
        
        public static WordManager Instance;
        
        [SerializeField] private RectTransform wordContainer;
        
        [Header("UI Slots (Order: W, O, N, D, E, R)")]
        [SerializeField] private List<Image> letterSlots;
        
        [Header("Sprites")]
        [SerializeField] private List<Sprite> activeLetterSprites;

        [Header("Animation")]
        [SerializeField] private GameObject flyingLetterPrefab;
        [SerializeField] private float flyDuration = 0.7f;

        private int _collectedCount;
        private bool[] _collectedFlags;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            
            _collectedFlags = new bool[letterSlots.Count];
        }
        
        #region Public API
        
        public void CollectLetter(ItemType type, Vector3 startWorldPos)
        {
            int index = GetIndexForType(type);
            if (index == -1 || _collectedFlags[index]) return;
            
            _collectedFlags[index] = true;
            AnimateLetterCollection(index, startWorldPos);
        }

        public bool IsLetter(ItemType type) => type >= ItemType.Letter_W && type <= ItemType.Letter_R;
        
        public List<ItemType> GetNeededLetterTypes()
        {
            List<ItemType> needed = new List<ItemType>();
            
            for (int i = 0; i < _collectedFlags.Length; i++)
            {
                if (!_collectedFlags[i])
                {
                    ItemType type = (ItemType)((int)ItemType.Letter_W + i);
                    needed.Add(type);
                }
            }
            
            return needed;
        }

        #endregion

        #region Internal Logic & Animation

        private void AnimateLetterCollection(int index, Vector3 startWorldPos)
        {
            GameObject flyingLetter = Instantiate(flyingLetterPrefab, wordContainer);
            flyingLetter.transform.position = Camera.main.WorldToScreenPoint(startWorldPos);
            
            Image img = flyingLetter.GetComponent<Image>();
            img.sprite = activeLetterSprites[index];
            
            Vector3 targetPos = letterSlots[index].transform.position;
            
            Sequence seq = DOTween.Sequence();
            seq.Append(flyingLetter.transform.DOMove(targetPos, flyDuration).SetEase(Ease.InBack));
            seq.Join(flyingLetter.transform.DOScale(Anim_Target_Scale, flyDuration));

            seq.OnComplete(() =>
            {
                Destroy(flyingLetter);
                OnLetterArrived(index);
            });
        }

        private void OnLetterArrived(int index)
        {
            // Activate slot
            letterSlots[index].sprite = activeLetterSprites[index];
            letterSlots[index].color = Color.white;
            
            // Punch effect
            letterSlots[index].transform.DOPunchScale(Vector3.one * Anim_Punch_Strength, Anim_Punch_Duration);
            
            CheckWinCondition();
        }

        private int GetIndexForType(ItemType type)
        {
            // Assuming Enum order is consecutive: W, O, N, D, E, R
            int offset = (int)type - (int)ItemType.Letter_W;

            if (offset >= 0 && offset < letterSlots.Count)
                return offset;
            
            return -1;
        }

        private void CheckWinCondition()
        {
            _collectedCount++;
            
            // Dynamic check based on list size (6)
            if (_collectedCount >= letterSlots.Count)
            {
                Debug.Log($"<color=green>WORD COMPLETED! ({_collectedCount}/{letterSlots.Count})</color>");
                
                // TODO: GameManager.Instance.WinLevel();
            }
        }

        #endregion
    }
}