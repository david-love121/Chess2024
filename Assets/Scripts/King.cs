using System.Collections.Generic;

using UnityEngine;

public class King : Piece
{
    public bool hasMoved;
    public override List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int testedXSquare = boardPosition.x + i;
                int testedYSquare = boardPosition.y + j;
                if (testedXSquare > 7 || testedXSquare < 0 || testedYSquare > 7 || testedYSquare < 0)
                {
                    continue;
                }
                Vector2Int move = new Vector2Int(boardPosition.x + i, boardPosition.y + j);
                if (move.Equals(boardPosition))
                {
                    continue;
                }
                if (!DetectCheckedSquare(move))
                {

                    moves.Add(move);
                }
            }
        }
        if (!hasMoved)
        {
            Vector2Int leftCorner = new Vector2Int(0, 0);
            Vector2Int rightCorner = new Vector2Int(7, 0);
            if (Color == Constants.Black)
            {
                leftCorner.y = 7;
                rightCorner.y = 7;
            }
            Piece left = GameController.GetFromBoard(leftCorner);
            Piece right = GameController.GetFromBoard(rightCorner);
            Vector2Int toCheck = new Vector2Int(BoardPosition.x - 1, BoardPosition.y);
            //Cast is safe as only Rooks meet the first condition
            if (left.PieceType == Constants.Rook && (left as Rook).hasMoved == false)
            {
                //Cannot castle through check
                if (GameController.GetFromBoard(toCheck) == null && !CanBeCaptured(toCheck))
                {
                    toCheck.x += -1;
                    if (GameController.GetFromBoard(toCheck) == null && !CanBeCaptured(toCheck))
                    {
                        moves.Add(toCheck);
                    }
                }
            }
            if (right.PieceType == Constants.Rook && (right as Rook).hasMoved == false)
            {
                toCheck.x = BoardPosition.x + 1;
                if (GameController.GetFromBoard(toCheck) == null && !CanBeCaptured(toCheck))
                {
                    toCheck.x += 1;
                    if (GameController.GetFromBoard(toCheck) == null && !CanBeCaptured(toCheck))
                    {
                        moves.Add(toCheck);
                    }
                }
            }

            
            
        }
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
    //Detects the first checked square found, returns true if checked
    internal bool DetectCheckedSquare(Vector2Int boardPosition)
    {

        return CanBeCaptured(boardPosition);
    }
    internal bool InCheck()
    {
        return DetectCheckedSquare(BoardPosition);
    }
    //Returns all pieces checking a king. Takes more time than DetectCheckedSquare
    internal List<Piece> DetectCheckingPieces(Vector2Int boardPosition)
    {
        List<Piece> pieces = new List<Piece>();
        Piece pieceAtPosition = null;
        List<Vector2Int> castXPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "x", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            pieces.Add(pieceAtPosition);
        }
        List<Vector2Int> castXNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "x", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            pieces.Add(pieceAtPosition);
        }
        List<Vector2Int> castYPos = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), 8, 1, "y", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            pieces.Add(pieceAtPosition);
        }
        List<Vector2Int> castYNeg = CastRayUntilPiece(new Vector2Int(boardPosition.x, boardPosition.y), -1, -1, "y", out pieceAtPosition);
        if (CheckPieceNullOrCheckingRQ(pieceAtPosition))
        {
            pieces.Add(pieceAtPosition);
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
                    pieces.Add(pieceAtPosition);
                }

            }
        }
        //Check for knight check
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
                //This block appears confusing, but is just calculating every position a knight can be at to check
                toCheck = new Vector2Int(boardPosition.x + (1 * shortMultiplier), boardPosition.y + (2 * longMultiplier));
                pieceAtPosition = GameController.GetFromBoard(toCheck);
                if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Knight)
                {
                    pieces.Add(pieceAtPosition);
                }
                toCheck = new Vector2Int(boardPosition.x + (2 * shortMultiplier), boardPosition.y + (1 * longMultiplier));
                pieceAtPosition = GameController.GetFromBoard(toCheck);
                if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Knight)
                {
                    pieces.Add(pieceAtPosition);
                }
            }
        }
        //Potentially could do some more abstraction here, considering this only happens in a very specific circumstance I will not
        //Check for pawn check
        if (this.Color == Constants.White)
        {
            toCheck = new Vector2Int(boardPosition.x + 1, boardPosition.y + 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                pieces.Add(pieceAtPosition);
            }
            toCheck = new Vector2Int(boardPosition.x - 1, boardPosition.y + 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                pieces.Add(pieceAtPosition);
            }
        }
        else
        {
            toCheck = new Vector2Int(boardPosition.x + 1, boardPosition.y - 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                pieces.Add(pieceAtPosition);
            }
            toCheck = new Vector2Int(boardPosition.x - 1, boardPosition.y - 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                pieces.Add(pieceAtPosition);
            }

        }
       
        
        return pieces;
    }

    
    
}