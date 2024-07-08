using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Pawn : Piece
{
    public bool HasMoved { get; private set; }
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        int colorMultiplier = Color == Constants.White ? 1 : -1;
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int moveToAdd = new Vector2Int(boardPosition.x, boardPosition.y + colorMultiplier);
        moves.Add(moveToAdd);
        if (!HasMoved)
        {
            moveToAdd.y = moveToAdd.y + colorMultiplier;
            moves.Add(moveToAdd);
        }
        Vector2Int leftCorner = new Vector2Int(boardPosition.x - 1, boardPosition.y + colorMultiplier);
        Vector2Int rightCorner = new Vector2Int(boardPosition.x + 1, boardPosition.y + colorMultiplier);
        Piece leftCornerPiece = GameController.GetFromBoard(leftCorner);
        Piece rightCornerPiece = GameController.GetFromBoard(rightCorner);
        if (leftCornerPiece != null)
        {
            if (leftCornerPiece.Color != Color)
            {
                moves.Add(leftCorner);
            }
        }
        if (rightCornerPiece != null)
        {
            if (rightCornerPiece.Color != Color)
            {
                moves.Add(rightCorner);
            }
        }

        return moves;
    }

    public override bool Move(Vector2Int newPosition)
    {
        HasMoved = true;

        return base.Move(newPosition);
    }

    public override void Start()
    {
        HasMoved = false;
        base.Start();

    }

    public override void Update()
    {
        base.Update();
    }
}