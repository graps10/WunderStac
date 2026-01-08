using UnityEngine;

namespace Game.Board
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public int X { get; private set; }
        public int Y { get; private set; }

        public void Init(int x, int y)
        {
            X = x;
            Y = y;
            gameObject.name = $"Tile_{x}_{y}";
        }
    }
}