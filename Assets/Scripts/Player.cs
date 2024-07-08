using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player
{
    
    public Queen Queen;
    public King King;
    public Pawn[] Pawns;
    public Knight[] Knights;
    public Bishop[] Bishops;
    public Rook[] Rooks;
    private PieceFactory PieceFactory;
    private GameController GameController;
    public string Color;

    public Player(PieceFactory PieceFactory, string Color, GameController GameController)
    {
        this.PieceFactory = PieceFactory;
        this.Color = Color;
        this.GameController = GameController;
        InitalizePieces();
    }
    public void InitalizePieces()
    {
        Pawns = new Pawn[8];
        Knights = new Knight[2];
        Bishops = new Bishop[2];
        Rooks = new Rook[2];
        int backRow = Color == Constants.White ? 0 : 7;
        int pawnRow = Color == Constants.White ? 1 : 6;
        for (int i = 0; i < 8; i++)
        {
            Vector2Int Position = new Vector2Int(i, pawnRow);
            Pawn pawn = PieceFactory.MakeNewPiece(Color, Position, Constants.Pawn, GameController) as Pawn;
            Pawns[i] = pawn;
        }
        Queen = PieceFactory.MakeNewPiece(Color, new Vector2Int(3, backRow), Constants.Queen, GameController) as Queen;
        King = PieceFactory.MakeNewPiece(Color, new Vector2Int(4, backRow), Constants.King, GameController) as King;
        Knights[0] = PieceFactory.MakeNewPiece(Color, new Vector2Int(1, backRow), Constants.Knight, GameController) as Knight;
        Knights[1] = PieceFactory.MakeNewPiece(Color, new Vector2Int(6, backRow), Constants.Knight, GameController) as Knight;
        Bishops[0] = PieceFactory.MakeNewPiece(Color, new Vector2Int(2, backRow), Constants.Bishop, GameController) as Bishop;
        Bishops[1] = PieceFactory.MakeNewPiece(Color, new Vector2Int(5, backRow), Constants.Bishop, GameController) as Bishop;
        Rooks[0] = PieceFactory.MakeNewPiece(Color, new Vector2Int(0, backRow), Constants.Rook, GameController) as Rook;
        Rooks[1] = PieceFactory.MakeNewPiece(Color, new Vector2Int(7, backRow), Constants.Rook, GameController) as Rook;

    }


}
