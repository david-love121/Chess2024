using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceFactory : MonoBehaviour
{
    public Grid Grid;
    public Tilemap Board;
    public GameObject WhitePawn;
    public GameObject WhiteBishop;
    public GameObject WhiteKnight;
    public GameObject WhiteRook;
    public GameObject WhiteKing;
    public GameObject WhiteQueen;
    public GameObject BlackPawn;
    public GameObject BlackBishop;
    public GameObject BlackKnight;
    public GameObject BlackRook;
    public GameObject BlackKing;
    public GameObject BlackQueen;
    public GameObject WhitePlaceholder;
    public GameObject BlackPlaceholder;
    //Creates a new piece and returns it
    public Piece MakeNewPiece(string color, Vector2Int position, string type, GameController gameController)
    {

        Piece newPiece;
        GameObject selectedPrefab = WhitePawn;
        if (color == Constants.White) {
            if (type == Constants.Bishop)
            {
                selectedPrefab = WhiteBishop;
            }
            if (type == Constants.Queen)
            {
                selectedPrefab = WhiteQueen;
            }
            if (type == Constants.King)
            {
                selectedPrefab = WhiteKing;
            }
            if (type == Constants.Pawn)
            {
                selectedPrefab = WhitePawn;
            }
            if (type == Constants.Rook)
            {
                selectedPrefab = WhiteRook;
            }
            if (type == Constants.Knight)
            {
                selectedPrefab = WhiteKnight;
            }
            if (type == Constants.Placeholder)
            {
                selectedPrefab = WhitePlaceholder;
            }
            
        } else if (color == Constants.Black)
        {
            if (type == Constants.Bishop)
            {
                selectedPrefab = BlackBishop;
            }
            if (type == Constants.Queen)
            {
                selectedPrefab = BlackQueen;
            }
            if (type == Constants.King)
            {
                selectedPrefab = BlackKing;
            }
            if (type == Constants.Pawn)
            {
                selectedPrefab = BlackPawn;
            }
            if (type == Constants.Rook)
            {
                selectedPrefab = BlackRook;
            }
            if (type == Constants.Knight)
            {
                selectedPrefab = BlackKnight;
            }
            if (type == Constants.Placeholder)
            {
                selectedPrefab = BlackPlaceholder;
            }
        } else
        {
            throw new System.ArgumentException("Invalid argument initalizing piece");
        }
        GameObject newGameObject = Instantiate(selectedPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        newPiece = newGameObject.GetComponent<Piece>();
        //Pieces have to be instantiated in a factoryClass because they need to exist as Unity GameObjects too
        newPiece.BoardPosition = position;
        newPiece.SetColor(color);
        newPiece.transform.SetParent(Grid.transform, false);
        Bounds boardBounds = Board.localBounds;
        Vector3 boardMin = boardBounds.min;
        Vector3 boardAnchor = Board.tileAnchor;
        newPiece.Tilemap = Board;
        newPiece.Move(position);
        newPiece.SetGameController(gameController);
        newPiece.UnityObject = newGameObject;
        newPiece.PieceType = type;
        gameController.InitalizePiece(newPiece);
        return newPiece; 
    }
}