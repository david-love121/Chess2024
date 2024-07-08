using System.Collections.Generic;
using UnityEngine;
//Only for computational purposes, never visible to player and has no Prefab
public class Placeholder : Piece
{
    //Has no possible moves, this is just for determining if a check can be broken
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<Vector2Int> emptyList = new List<Vector2Int>();
        return emptyList;
    }
    //Only this piece has an initalizer as it doesn't come ferom PieceFactory
    public Placeholder(string color, Vector2Int boardPosition, GameController gameController)  
    {
        this.SetColor(color);
        BoardPosition = boardPosition;
        GameController = gameController;
    }
    //Checks if something other than a king or a pawn can take a piece. For usage when checking win condition
    internal bool PreventsCheck()
    {
        Vector2Int boardPosition = BoardPosition;
        Piece pieceAtPosition = null;
        List<Vector2Int> castXPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "x", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            return true;
        }
        List<Vector2Int> castXNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "x", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            return true;
        }
        List<Vector2Int> castYPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "y", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            return true;
        }
        List<Vector2Int> castYNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "y", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            return true;
        }
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

                List<Vector2Int> DiagonalRay = CastDiagonalRay(boardPosition, xDirection, yDirection, out pieceAtPosition);
                if (CheckPieceNullOrCheckingBQ(pieceAtPosition))
                {
                    return true;
                }

            }
        }
        //Check for knight 
        Vector2Int toCheck;
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
                //This block appears confusing, but is just calculating every position a knight can be at 
                toCheck = new Vector2Int(boardPosition.x + (1 * shortMultiplier), boardPosition.y + (2 * longMultiplier));
                pieceAtPosition = GameController.GetFromBoard(toCheck);
                if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Knight)
                {
                    return true;
                }
                toCheck = new Vector2Int(boardPosition.x + (2 * shortMultiplier), boardPosition.y + (1 * longMultiplier));
                pieceAtPosition = GameController.GetFromBoard(toCheck);
                if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Knight)
                {
                    return true;
                }
            }
        }
        int colorMultiplier = Color == Constants.White ? 1 : -1;
        Vector2Int pos = new Vector2Int(boardPosition.x, boardPosition.y + colorMultiplier);
        pieceAtPosition = GameController.GetFromBoard(pos);
        if (pieceAtPosition != null && pieceAtPosition.PieceType == Constants.Pawn)
        {
            return true;
        }
        pos.Set(boardPosition.x, boardPosition.y + (2*colorMultiplier));
        if (pieceAtPosition != null && pieceAtPosition.PieceType == Constants.Pawn && !(pieceAtPosition as Pawn).HasMoved)
        {
            return true;
        }
        return false;
    }
}