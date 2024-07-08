using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int longMultiplier = 1;
        int shortMultiplier = 1;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                longMultiplier = -1;
                shortMultiplier = 1;
            }
            for (int k = 0; k < 2; k++)
            {
                if (k == 1)
                {
                    shortMultiplier = -1;
                }
                Vector2Int moveToAdd = new Vector2Int(boardPosition.x + (1 * shortMultiplier), boardPosition.y + (2 * longMultiplier));
                Vector2Int secondMoveToAdd = new Vector2Int(boardPosition.x + (2 * shortMultiplier), boardPosition.y + (1 * longMultiplier));
                if (Piece.InBounds(moveToAdd))
                {
                    moves.Add(moveToAdd);
                }
                if (Piece.InBounds(secondMoveToAdd))
                { 
                    moves.Add(secondMoveToAdd);
                }
            }
        }
        return moves;
        
    }
}
