using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Queen : Piece
{
    
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<List<Vector2Int>> raycastLists = new List<List<Vector2Int>>();
        //less than 8 because the board is indexed 0-7
        List<Vector2Int> castXPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "x");
        raycastLists.Add(castXPos);
        List<Vector2Int> castXNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "x");
        raycastLists.Add(castXNeg);
        List<Vector2Int> castYPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "y");
        raycastLists.Add(castYPos);
        List<Vector2Int> castYNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "y");
        raycastLists.Add(castYNeg);
        //Diagonals

        int xDirection = 1;
        int yDirection = 1;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                xDirection = -1;
                yDirection = 1;
            }
            for (int k = 0; k < 2; k++)
            {
               if (k == 1)
               {
                    yDirection = -1;
               }
                List<Vector2Int> DiagonalRay = CastDiagonalRay(boardPosition, xDirection, yDirection);
                raycastLists.Add(DiagonalRay);
                
            }
        }
        List<Vector2Int> moves = raycastLists.SelectMany(x => x).ToList();

        return moves;
    }
}