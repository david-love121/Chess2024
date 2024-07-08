using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<List<Vector2Int>> raycastLists = new List<List<Vector2Int>>();

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