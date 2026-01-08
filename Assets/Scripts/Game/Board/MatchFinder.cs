using System.Collections.Generic;

namespace Game.Board
{
    public static class MatchFinder
    {
        public static List<GamePiece> FindMatches(GamePiece[,] board, int width, int height)
        {
            HashSet<GamePiece> matchedPieces = new HashSet<GamePiece>();

            // Horizontal Check
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GamePiece current = board[x, y];
                    if (current == null) continue;

                    if (x + 2 < width)
                    {
                        GamePiece right1 = board[x + 1, y];
                        GamePiece right2 = board[x + 2, y];

                        if (right1 != null && right2 != null && 
                            current.Type == right1.Type && current.Type == right2.Type)
                        {
                            matchedPieces.Add(current);
                            matchedPieces.Add(right1);
                            matchedPieces.Add(right2);
                        }
                    }
                }
            }

            // Vertical Check
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GamePiece current = board[x, y];
                    if (current == null) continue;

                    if (y + 2 < height)
                    {
                        GamePiece up1 = board[x, y + 1];
                        GamePiece up2 = board[x, y + 2];

                        if (up1 != null && up2 != null && 
                            current.Type == up1.Type && current.Type == up2.Type)
                        {
                            matchedPieces.Add(current);
                            matchedPieces.Add(up1);
                            matchedPieces.Add(up2);
                        }
                    }
                }
            }

            return new List<GamePiece>(matchedPieces);
        }
    }
}