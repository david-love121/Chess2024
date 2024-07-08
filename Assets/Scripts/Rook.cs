using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rook : Piece
{
    public bool hasMoved;
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<List<Vector2Int>> raycastLists = new List<List<Vector2Int>>();
        //less than 8 because the board is indexed 0-7
        List<Vector2Int> castXPos = CastRayUntilPiece(new Vector2Int(boardPosition.x + 1, boardPosition.y), 8, 1, "x");
        raycastLists.Add(castXPos);
        List<Vector2Int> castXNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x - 1, boardPosition.y), -1, -1, "x");
        raycastLists.Add(castXNeg);
        List<Vector2Int> castYPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y + 1), 8, 1, "y");
        raycastLists.Add(castYPos);
        List<Vector2Int> castYNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y - 1), -1, -1, "y");
        raycastLists.Add(castYNeg);
        List<Vector2Int> moves = raycastLists.SelectMany(x => x).ToList();
        return moves;
    }
    public override bool Move(Vector2Int newPosition)
    {
        hasMoved = true;
        return base.Move(newPosition);
    }
    public override void Start()
    {
        hasMoved = false;
        base.Start();
    }
}
