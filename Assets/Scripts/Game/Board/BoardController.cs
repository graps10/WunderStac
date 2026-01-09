using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI.Game;
using UnityEngine;

namespace Game.Board
{
    public class BoardController : MonoBehaviour
    {
        private const float Fill_Delay = 0.2f;
        private const float Destroy_Delay = 0.2f;
        private const float Letters_Collect_Delay = 0.3f;
        private const int Max_Gen_Attempts = 100;
        // Layout Constants
        private const float Cell_Size = 0.48f; 
        private const int Spawn_Y_Offset = 2;
        
        [Header("Layout Settings")] 
        [SerializeField] private int width = 6;
        [SerializeField] private int height = 7;
        [SerializeField] private float spacing = 0.15f; 
        [SerializeField] private float boardYOffset;
        
        [Header("Generation Logic")]
        [SerializeField] private PieceProvider pieceProvider;
        
        [Header("Input Settings")]
        [SerializeField] private float swapDuration = 0.25f;
        [SerializeField] private float minSwipeDistance = 0.5f;

        [Header("Prefabs")]
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private GamePiece piecePrefab;

        [Header("Data")]
        [SerializeField] private List<Sprite> pieceSprites; 
        
        private GamePiece[,] _allPieces;
        private Tile[,] _allTiles;
        
        private int _comboMultiplier;
        
        private GamePiece _selectedPiece;
        private Vector2 _startTouchPos;
        private Vector2 _endTouchPos;
        private bool _isSwapping;

        private void Start()
        {
            pieceProvider ??= new PieceProvider(); //  for safety
            
            _allPieces = new GamePiece[width, height];
            _allTiles = new Tile[width, height];

            GenerateBoard();
        }
        
        private void Update()
        {
            if (_isSwapping || !GameManager.Instance.IsGameActive) 
                return;
            
            HandleInput();
        }

        #region Input Logic
        
        private void HandleInput()
        {
            // Touch Start
            if (Input.GetMouseButtonDown(0))
            {
                _startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(_startTouchPos, Vector2.zero);
                
                if (hit.collider != null)
                    _selectedPiece = hit.collider.GetComponent<GamePiece>();
            }
            
            // 2. Touch End (Swipe)
            if (Input.GetMouseButtonUp(0) && _selectedPiece != null)
            {
                _endTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                
                if (Vector2.Distance(_startTouchPos, _endTouchPos) > minSwipeDistance)
                    CalculateSwipeDirection();
                
                _selectedPiece = null;
            }
        }
        
        private void CalculateSwipeDirection()
        {
            float angle = Mathf.Atan2(_endTouchPos.y - _startTouchPos.y, _endTouchPos.x - _startTouchPos.x) * 180 / Mathf.PI;
            
            // Normalize angle to directions
            int xDir = 0, yDir = 0;
            
            if (angle > -45 && angle <= 45) xDir = 1;          // Right
            else if (angle > 45 && angle <= 135) yDir = 1;     // Up
            else if (angle > 135 || angle <= -135) xDir = -1;  // Left
            else if (angle < -45 && angle >= -135) yDir = -1;  // Down

            TrySwap(_selectedPiece.X, _selectedPiece.Y, xDir, yDir);
        }
        
        #endregion
        
        #region Core Gameplay (Swap & Match)
        
        private void TrySwap(int x, int y, int xOffset, int yOffset)
        {
            int targetX = x + xOffset;
            int targetY = y + yOffset;
            
            // Check Bounds
            if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height) return;

            GamePiece targetPiece = _allPieces[targetX, targetY];
            GamePiece currentPiece = _allPieces[x, y];

            if (targetPiece != null && currentPiece != null)
                StartCoroutine(SwapPiecesRoutine(currentPiece, targetPiece));
        }
        
        private IEnumerator SwapPiecesRoutine(GamePiece pieceA, GamePiece pieceB)
        {
            _isSwapping = true;
            _comboMultiplier = 0;
            
            // Data Swap
            int xA = pieceA.X; int xB = pieceB.X;
            int yA = pieceA.Y; int yB = pieceB.Y;
            
            _allPieces[xA, yA] = pieceB;
            _allPieces[xB, yB] = pieceA;
            
            pieceA.SetPosition(xB, yB); 
            pieceB.SetPosition(xA, yA);
            
            Vector3 posA = pieceA.transform.position;
            Vector3 posB = pieceB.transform.position;

            // Visual Swap
            Sequence swapSeq = DOTween.Sequence();
            swapSeq.Join(pieceA.MoveToPosition(posB, swapDuration));
            swapSeq.Join(pieceB.MoveToPosition(posA, swapDuration));

            yield return swapSeq.WaitForCompletion();

            var matches = MatchFinder.FindMatches(_allPieces, width, height);

            if (matches.Count > 0)
            {
                // Valid Move
                GameManager.Instance?.ConsumeMove();
                yield return StartCoroutine(ProcessMatchesRoutine(matches));
                //Debug.Log($"Match Found! Count: {matches.Count}");
            }
            else
            {
                // Invalid Move -> Swap Back
                // Data Swap Back
                _allPieces[xA, yA] = pieceA;
                _allPieces[xB, yB] = pieceB;
                pieceA.SetPosition(xA, yA);
                pieceB.SetPosition(xB, yB);
                
                // Visual Swap Back
                Sequence backSeq = DOTween.Sequence();
                backSeq.Join(pieceA.MoveToPosition(posA, swapDuration));
                backSeq.Join(pieceB.MoveToPosition(posB, swapDuration));
                yield return backSeq.WaitForCompletion();
                
                _isSwapping = false;
            }
        }

        private IEnumerator ProcessMatchesRoutine(List<GamePiece> matches)
        {
            _comboMultiplier++;
            
            Vector3 centerPos = Vector3.zero;
            foreach (var p in matches) centerPos += p.transform.position;
            centerPos /= matches.Count;
            
            if (_comboMultiplier > 1)
            {
                GameEvents.OnComboUpdated?.Invoke(_comboMultiplier);
                
                string praise = GetComboText(_comboMultiplier);
                GameManager.Instance.ShowComboText(praise, centerPos);
            }
            
            HashSet<GamePiece> piecesToDestroy = new HashSet<GamePiece>(matches);
            
            foreach (var piece in matches)
                CheckForLettersAround(piece.X, piece.Y, piecesToDestroy);
            
            List<GamePiece> normalPieces = new List<GamePiece>();
            List<GamePiece> letters = new List<GamePiece>();

            foreach (var p in piecesToDestroy)
            {
                if (p == null) continue;
                
                _allPieces[p.X, p.Y] = null;

                if (WordManager.Instance != null && WordManager.Instance.IsLetter(p.Type))
                    letters.Add(p);
                else
                    normalPieces.Add(p);
            }
            
            foreach (var p in normalPieces)
            {
                p.DestroyPiece();
                if (GameManager.Instance != null)
                    GameManager.Instance.AddScore(GameManager.Instance.GetCurrentProfile().scorePerPiece);
            }
            
            if (letters.Count > 0)
                yield return new WaitForSeconds(Letters_Collect_Delay);
            
            foreach (var l in letters)
            {
                if (WordManager.Instance != null)
                    WordManager.Instance.CollectLetter(l.Type, l.transform.position);

                l.DestroyPiece();
                
                if (GameManager.Instance != null)
                    GameManager.Instance.AddScore(GameManager.Instance.GetCurrentProfile().scorePerLetter);
            }
            
            yield return new WaitForSeconds(Destroy_Delay);
            
            StartCoroutine(FillBoardRoutine());
        }
        
        #endregion

        #region Refill Logic
        
        private IEnumerator FillBoardRoutine()
        {
            //Debug.Log($"[FILL] Waiting for Destroy_Delay ({Destroy_Delay}s)... Time: {Time.time}");
            yield return new WaitForSeconds(Destroy_Delay);
            
            //Debug.Log($"[FILL] Starting Refill... Time: {Time.time}");
            yield return RefillBoard();
            //Debug.Log($"[FILL] Refill Finished (Visuals done). Time: {Time.time}");
            
            //Debug.Log($"[FILL] Waiting Human Delay ({Fill_Delay}s)...");
            yield return new WaitForSeconds(Fill_Delay);
            
            var matches = MatchFinder.FindMatches(_allPieces, width, height);
            if (matches.Count > 0)
            {
                //Debug.Log($"<color=cyan>[CASCADE] Found {matches.Count} new matches after fall! Time: {Time.time}</color>");
                StartCoroutine(ProcessMatchesRoutine(matches));
            }
            else
            {
                //Debug.Log("[TURN END] No more matches. Input Unlocked.");
                GameManager.Instance.OnTurnCompleted();
                if (GameManager.Instance.IsGameActive && GameManager.Instance.MovesLeft > 0)
                    _isSwapping = false; 
            }
        }

        private IEnumerator RefillBoard()
        {
            List<GamePiece> movedPieces = new List<GamePiece>();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_allPieces[x, y] == null)
                    {
                        // Find the nearest piece above
                        int ySearch = y + 1;
                        while (ySearch < height && _allPieces[x, ySearch] == null)
                        {
                            ySearch++;
                        }
                        
                        // Either move it down OR spawn new
                        if (ySearch < height)
                        {
                            GamePiece pieceToFall = _allPieces[x, ySearch];
                            
                            _allPieces[x, ySearch] = null;
                            _allPieces[x, y] = pieceToFall;
                            pieceToFall.SetPosition(x, y);
                            
                            movedPieces.Add(pieceToFall);
                            pieceToFall.MoveToPosition(_allTiles[x, y].transform.position, swapDuration);
                        }
                        else
                        {
                            ItemType type = pieceProvider.GetNextType();
                            Vector2 spawnPos = new Vector2(
                                _allTiles[x, y].transform.position.x, 
                                height + Spawn_Y_Offset
                            );
                            
                            CreatePieceAt(x, y, spawnPos, type);
                            GamePiece newPiece = _allPieces[x, y];
                            
                            movedPieces.Add(newPiece);
                            newPiece.MoveToPosition(_allTiles[x, y].transform.position, swapDuration);
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(swapDuration);
        }
        
        #endregion
        
        #region #region Generation & Helpers
        
        private void GenerateBoard() // Magic Numbers
        {
            float step = Cell_Size + spacing;
            float startX = -((width - 1) * step) / 2f;
            float startY = -((height - 1) * step) / 2f + boardYOffset;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 position = new Vector2(
                        startX + (x * step), 
                        startY + (y * step)
                    );
                    
                    // Create Tile Background
                    Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                    tile.Init(x, y);
                    _allTiles[x, y] = tile;
                    
                    int attempts = 0;
                    ItemType randomType;
                    
                    do
                    {
                        randomType = pieceProvider.GetRandomNormalType();
                    } 
                    while (HasMatchAt(x, y, randomType) && attempts++ < Max_Gen_Attempts); 

                    CreatePieceAt(x, y, position, randomType);
                }
            }
        }
        
        private void CreatePieceAt(int x, int y, Vector2 position, ItemType type)
        {
            Sprite sprite = pieceSprites[(int)type]; 
            GamePiece piece = Instantiate(piecePrefab, position, Quaternion.identity, transform);
            piece.Init(x, y, type, sprite);
            _allPieces[x, y] = piece;
        }
        
        private bool HasMatchAt(int x, int y, ItemType type)
        {
            // Left Check
            if (x >= 2)
            {
                if (_allPieces[x - 1, y].Type == type && _allPieces[x - 2, y].Type == type) return true;
            }
            // Bottom Check
            if (y >= 2)
            {
                if (_allPieces[x, y - 1].Type == type && _allPieces[x, y - 2].Type == type) return true;
            }
            
            return false;
        }
        
        private void CheckForLettersAround(int x, int y, HashSet<GamePiece> piecesToDestroy)
        {
            CheckNeighbor(x + 1, y, piecesToDestroy); // Right
            CheckNeighbor(x - 1, y, piecesToDestroy); // Left
            CheckNeighbor(x, y + 1, piecesToDestroy); // Top
            CheckNeighbor(x, y - 1, piecesToDestroy); // Bottom
        }
        
        private void CheckNeighbor(int x, int y, HashSet<GamePiece> set)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;

            GamePiece neighbor = _allPieces[x, y];
            if (neighbor != null && WordManager.Instance.IsLetter(neighbor.Type))
            {
                if (!set.Contains(neighbor))
                {
                    //Debug.Log($"<color=orange>[NEIGHBOR HIT] Found Letter {neighbor.Type} at [{x}, {y}]. It was neighbor to a match!</color>");
                    set.Add(neighbor);
                }
            }
        }
        
        private string GetComboText(int combo)
        {
            switch (combo)
            {
                case 2: return "Good!";
                case 3: return "Excellent!";
                default: return $"Combo x{combo}!";
            }
        }

        #endregion
        
    }
}