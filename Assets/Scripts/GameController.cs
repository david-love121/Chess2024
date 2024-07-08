using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController
{
    public string TurnColor { get; private set; }
    public int TurnNumber { get; private set; }
    private Piece[,] Board { get; set; }
    internal bool WhiteInCheckFlag { get; private set; }
    internal bool BlackInCheckFlag { get; private set; }
    internal bool GameEnded { get; private set; }
    public string WinningPlayer { get; private set; }
    private King WhiteKing;
    private King BlackKing;
    private Piece WhiteCheckingPiece;

    private Piece BlackCheckingPiece;
    private PieceFactory PieceFactory;
    public GameController(PieceFactory pieceFactory)
    {
        TurnColor = Constants.White;
        TurnNumber = 0;
        Board = new Piece[8,8];
        WhiteInCheckFlag = false;
        BlackInCheckFlag = false;
        WinningPlayer = string.Empty;
        GameEnded = false;
        PieceFactory = pieceFactory;
    }
    
    public string GetTurnColor()
    {
        return TurnColor;
    }
    public int GetTurnNumber()
    {
        return TurnNumber;
    }
    public Piece[,] GetBoard()
    {
        return Board;
    }
    public Piece GetFromBoard(Vector2Int position)
    {
        if (position.x < 0 || position.x > 7 || position.y < 0 || position.y > 7)
        {
            //Debug.Log("Warning: A piece just attempted to check the piece at a position out of bounds.");
            return null;

        }
        return Board[position.x, position.y];
    }
    public Piece GetFromBoard(int x, int y)
    {
        return Board[x, y];
    }

    //Returns true if the move is possible and completed, false if not

    public bool MakeMove(Piece selectedPiece, Vector2Int desiredPosition)
    {
        if (selectedPiece.Color != TurnColor)
        {
            throw new System.Exception("A player is trying to move outside of their turn!");
        }
        
        //Check for castle
        if (selectedPiece.PieceType == Constants.King)
        {
            int difference = selectedPiece.BoardPosition.x - desiredPosition.x;
            int rank = selectedPiece.Color == Constants.White ? 0 : 7;
            if (Math.Abs(difference) > 1)
            {
                //Moving to the queen side
                if (difference > 0)
                {
                    UpdateBoard(new Vector2Int(0, rank), new Vector2Int(3, rank), out Piece movedRook);
                    movedRook.Move(new Vector2Int(3, rank));

                } else
                {
                    UpdateBoard(new Vector2Int(7, rank), new Vector2Int(5, rank), out Piece movedRook);
                    movedRook.Move(new Vector2Int(5, rank));
                }
            }
        }
        
        UpdateBoard(selectedPiece.BoardPosition, desiredPosition);
        selectedPiece.Move(desiredPosition);
        IncrementTurn();
        UpdateCheckFlags();
        if (WhiteInCheckFlag || BlackInCheckFlag)
        {
            if (CheckWinCondition())
            {
                Debug.Log(WinningPlayer + " has won");
            };
        }
        return true;

    }
    //Checks if a move can be made, Does not check for move legality based on how a piece moves, piece class does this. 
    //Checks based On other pieces
    public bool MoveIsPossible(Piece selectedPiece, Vector2Int desiredPosition)
    {
        //Save the board before modifying it to check legality
        if (WhiteKing == null || BlackKing == null)
        {
            foreach (Piece piece in Board)
            {
                if (piece != null && piece.Color == Constants.White && piece.PieceType == Constants.King)
                {
                    WhiteKing = piece as King;
                }
                if (piece != null && piece.Color == Constants.Black && piece.PieceType == Constants.King)
                {
                    BlackKing = piece as King;
                }
            }
        }
        Piece pieceAtDesiredPos = Board[desiredPosition.x, desiredPosition.y];
        Piece[,] lastBoard = new Piece[8, 8];
        Array.Copy(Board, lastBoard, Board.Length);
        //Player can only move out of check in check
        if (selectedPiece.Color == Constants.White && WhiteInCheckFlag)
        {
            
            UpdateBoard(selectedPiece.BoardPosition, desiredPosition);
            if (WhiteKing.InCheck())
            {
                //If the player will still be in check, return false
                Array.Copy(lastBoard, Board, Board.Length);
                return false;
            }
            Array.Copy(lastBoard, Board, Board.Length);
        }
        if (selectedPiece.Color == Constants.Black && BlackInCheckFlag)
        {

            UpdateBoard(selectedPiece.BoardPosition, desiredPosition);
            if (BlackKing.InCheck())
            {
                Array.Copy(lastBoard, Board, Board.Length);
                return false;
            }
            
        }
        
        UpdateBoard(selectedPiece.BoardPosition, desiredPosition);
        King currentPlayerKing = TurnColor == Constants.White ? WhiteKing : BlackKing;
        if (currentPlayerKing.InCheck())
        {
            //A player cannot move into check
            Array.Copy(lastBoard, Board, Board.Length);
            return false;
        }
        
        //Add if piece is typeof king
        if (pieceAtDesiredPos != null)
        {
            if (pieceAtDesiredPos.Color == TurnColor)
            {
                Array.Copy(lastBoard, Board, Board.Length);
                return false; //Player cannot move on their own piece
            }

            pieceAtDesiredPos.DestroyPiece();

        }
        Array.Copy(lastBoard, Board, Board.Length);
        return true;

    }
    public bool CheckWinCondition()
    {
        King checkedKing;
        string playerColor;
        if (WhiteInCheckFlag) 
        {
            checkedKing = WhiteKing;
            playerColor = Constants.Black;
        } else
        {
            checkedKing = BlackKing;
            playerColor = Constants.White;
        }
        //Check if the king can move out of check
        List<Vector2Int> possibleMoves = checkedKing.CalculatePossibleMoves(checkedKing.BoardPosition);
        foreach (Vector2Int possibleMove in possibleMoves)
        {
            
            if (MoveIsPossible(checkedKing, possibleMove))
            {
                return false;
            }
        }
        //Check if a piece can prevent the check
        List<Piece> checkingPieces = checkedKing.DetectCheckingPieces(checkedKing.BoardPosition);
        //Only a king move can prevent a double check, skip searching for captures or blocks
        List<Vector2Int> possibleSquares = new List<Vector2Int>();
        if (checkingPieces.Count <= 1)
        {
            Piece checkingPiece = checkingPieces[0];
            if (checkingPiece.CanBeCaptured())
            {
                return false;
            }
            //Direction of the ray fired by the king
            int xDirection;
            int yDirection;
            Vector2Int origin = checkedKing.BoardPosition;
            if (checkingPiece.BoardPosition.x < origin.x)
            {
                xDirection = -1;
            } else if (checkingPiece.BoardPosition.x > origin.x)
            {
                xDirection = 1;
            } else
            {
                xDirection = 0;
            }
            if (checkingPiece.BoardPosition.y < origin.y)
            {
                yDirection = -1;
            }
            else if (checkingPiece.BoardPosition.y > origin.y)
            {
                yDirection = 1;
            }
            else
            {
                yDirection = 0;
            }
            while (origin.x != checkingPiece.BoardPosition.x || origin.y != checkingPiece.BoardPosition.y)
            {
                if (origin.x != checkingPiece.BoardPosition.x)
                {
                    origin.x += xDirection;
                }
                if (origin.y != checkingPiece.BoardPosition.y)
                {
                    origin.y += yDirection;
                }
                possibleSquares.Add(origin);
            }

        }
        Piece[,] lastBoard = new Piece[8, 8];
        Vector2Int currentBoardPosition = possibleSquares[0];
        Placeholder placeholder = PieceFactory.MakeNewPiece(playerColor, currentBoardPosition, Constants.Placeholder, this) as Placeholder;
        Array.Copy(Board, lastBoard, Board.Length);
        //Minus one to not check the sqaure with the checking piece
        for (int i = 0; i < possibleSquares.Count - 1; i++)
        {
            currentBoardPosition = possibleSquares[i];
            UpdateBoard(placeholder.BoardPosition, currentBoardPosition);
            placeholder.BoardPosition = currentBoardPosition;
            if (placeholder.PreventsCheck())
            {
                //If the player will not be in check, return false game is not over
                Array.Copy(lastBoard, Board, Board.Length);
                placeholder.DestroyPiece();
                return false;
            }
        }
        placeholder.DestroyPiece();
        Array.Copy(lastBoard, Board, Board.Length);
        WinningPlayer = playerColor;
        GameEnded = true;
        return true;
    }
    
    //Only for usage when initalizing game or checking win condition
    public void InitalizePiece(Piece piece)
    {
        Board[piece.BoardPosition.x, piece.BoardPosition.y] = piece;
        return;
    }
    public void DeletePiece(Vector2Int boardPosition)
    {
        Board[boardPosition.x, boardPosition.y] = null;
        return;
    }

    private void IncrementTurn()
    {
        TurnNumber++;
        if (TurnColor == Constants.White)
        {
            TurnColor = Constants.Black;
        } else
        {
            TurnColor = Constants.White;
        }
        
        return;

    }
    private void UpdateCheckFlags()
    {
        if (WhiteKing == null || BlackKing == null)
        {
            foreach (Piece piece in Board)
            {
                if (piece != null && piece.Color == Constants.White && piece.PieceType == Constants.King)
                {
                    WhiteKing = piece as King;
                }
                if (piece != null && piece.Color == Constants.Black && piece.PieceType == Constants.King)
                {
                    BlackKing = piece as King;
                }
            }
        }
        if (WhiteKing.DetectCheckedSquare(WhiteKing.BoardPosition))
        {
            WhiteInCheckFlag = true;
        } else
        {
            WhiteInCheckFlag = false;
        }
        if (BlackKing.DetectCheckedSquare(BlackKing.BoardPosition))
        {
            BlackInCheckFlag = true;
        } else
        {
            BlackInCheckFlag = false;
        }
    }
    private void UpdateBoard(Vector2Int oldPosition, Vector2Int newPosition)
    {
        UpdateBoard(oldPosition, newPosition, out _);
    }
    private void UpdateBoard(Vector2Int oldPosition, Vector2Int newPosition, out Piece movedPiece)
    {
        movedPiece = Board[oldPosition.x, oldPosition.y];
        if (oldPosition == newPosition)
        {
            return;
        }
        Board[newPosition.x, newPosition.y] = movedPiece;
        Board[oldPosition.x, oldPosition.y] = null;
    }
}