using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Piece : MonoBehaviour
{
    //Initalized by PieceFactory
    public Tilemap Tilemap;
    public string Color { get; private set; }
    public Vector2Int BoardPosition {  get; set; }
    public GameObject UnityObject { get; set; }
    private bool Dragging;
    private Camera Cam;
    protected GameController GameController;
    public string PieceType {  get; set; }
    public virtual bool Move(Vector2Int newPosition)   
    {
        BoardPosition = newPosition;
        MoveToBoardPosition(BoardPosition);
        return true;
        

    }
    public void SetColor(string color)
    {
        Color = color;
    }
    public virtual void Start() 
    { 
        Dragging = false;
        Cam = Camera.main;
    }
    public virtual void Update() { }
    //Piece is clicked and drug by player
    public void OnMouseDrag()
    {
        //Piece can only be drug if it is that player's turn
        if (Color == GameController.TurnColor)
        {
            Dragging = true;
            Vector3 worldPosition = GetMousePositionAsWorld();
            this.transform.position = worldPosition;
        }
        //0 indexed
        
    }
    //Piece is dropped by player
    public void OnMouseUp()
    {
        
        if (Dragging == true)
        {
            Vector3 worldPosition = GetMousePositionAsWorld();
            Vector3Int cellPosition = Tilemap.WorldToCell(worldPosition);
            Vector2Int newBoardPosition = new Vector2Int(cellPosition.x + 4, cellPosition.y + 4);

            List<Vector2Int> possibleMoves = CalculatePossibleMoves(BoardPosition);
            Dragging = false;
            Vector3 localPosition = Tilemap.WorldToLocal(worldPosition);
            if (!PlacedOnTile(localPosition))
            {
                //Return to original position, cancel move
                MoveToBoardPosition(BoardPosition);
                return;
            }
            if (IsValidMove(possibleMoves, newBoardPosition))
            {
                
                return;
            } else
            {
                MoveToBoardPosition(BoardPosition);
                return;
            }
        }
    }
    public abstract List<Vector2Int> CalculatePossibleMoves(Vector2Int boardPosition);
    public bool IsValidMove(List<Vector2Int> possibleMoves, Vector2Int desiredMove)
    {
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            if (possibleMoves[i].Equals(desiredMove))
            {
                if (GameController.MoveIsPossible(this, desiredMove))
                {

                    if (GameController.MakeMove(this, desiredMove))
                    {
                        return true;
                    }
                }
                
            }
        }
        return false;
    }
    public bool PlacedOnTile(Vector3 localPosition)
    {
        //Decimal of the two numbers
        float remainderX = Math.Abs(localPosition.x % 1);
        float remainderY = Math.Abs(localPosition.y % 1);

        if (remainderX <= 0.15 || remainderX >= 0.85 || remainderY <= 0.15 || remainderY >= 0.85)
        {
            Debug.Log("bad move");
            return false;
        }
        return true;
    }
    public Vector3 GetMousePositionAsWorld()
    {
        Vector3 MousePosition = Input.mousePosition;
        Vector3 WorldPosition = Cam.ScreenToWorldPoint(MousePosition);
        WorldPosition.z = 0;
        return WorldPosition;
    }
    public void MoveToBoardPosition(Vector2Int boardPosition)
    {
        Vector3 localPosition = new Vector3(boardPosition.x - 3.5f, boardPosition.y - 3.5f);
        this.transform.localPosition = localPosition;
    }
    public void SetGameController(GameController gameController)
    {
        GameController = gameController;
    }
    public void DestroyPiece()
    {
        Destroy(UnityObject);
        Destroy(this);
        
    }
    protected List<Vector2Int> CastRayUntilPiece(Vector2Int startPosition, int bound, int direction, string variable)
    {
        return CastRayUntilPiece(startPosition, bound, direction, variable, out _);
    }
    protected List<Vector2Int> CastRayUntilPiece(Vector2Int startPosition, int bound, int direction, string variable, out Piece pieceAtPosition)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int toIncrement;
        if (variable.Equals("x"))
        {
            toIncrement = startPosition.x;
        } else if (variable.Equals("y"))
        {
            toIncrement = startPosition.y;
        } else
        {
            throw new ArgumentException("Not a valid axis!");
        }
        pieceAtPosition = null;
        toIncrement += direction;
        //Ray casting, ends when the loop hits end of board or another piece
        //The piece is included in the move list to allow for capture, GameController checks if they can
        while (WithinBound(toIncrement, bound, direction) && pieceAtPosition == null)
        {
            Vector2Int moveToAdd;
            if (variable.Equals("x"))
            {
                 moveToAdd = new Vector2Int(toIncrement, startPosition.y);
            } else
            {
                moveToAdd = new Vector2Int(startPosition.x, toIncrement);
            }
            pieceAtPosition = GameController.GetFromBoard(moveToAdd);
            toIncrement += direction;
            moves.Add(moveToAdd);
        }
        return moves;
    }
    protected List<Vector2Int> CastDiagonalRay(Vector2Int startPosition, int xDirection, int yDirection)
    {
        return CastDiagonalRay(startPosition, xDirection, yDirection, out _);
    }
    protected List<Vector2Int> CastDiagonalRay(Vector2Int startPosition, int xDirection, int yDirection, out Piece pieceAtPosition)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        pieceAtPosition = null;
        int x = startPosition.x + xDirection;
        int y = startPosition.y + yDirection;
        while (x > -1 && y > -1 && x < 8 && y < 8 && pieceAtPosition == null)
        {
            Vector2Int moveToAdd = new Vector2Int(x, y);
            pieceAtPosition = GameController.GetFromBoard(moveToAdd);
            x += xDirection;
            y += yDirection;
            moves.Add(moveToAdd);
        }
        return moves;
    }
    internal bool CanBeCaptured()
    {
        return CanBeCaptured(BoardPosition);
    }
    internal bool CanBeCaptured(Vector2Int boardPosition)
    {
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
        //Potentially could do some more abstraction here, considering this only happens in a very specific circumstance I will not
        //Check for pawn 
        if (this.Color == Constants.White)
        {
            toCheck = new Vector2Int(boardPosition.x + 1, boardPosition.y + 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                return true;
            }
            toCheck = new Vector2Int(boardPosition.x - 1, boardPosition.y + 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                return true;
            }
        }
        else
        {
            toCheck = new Vector2Int(boardPosition.x + 1, boardPosition.y - 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                return true;
            }
            toCheck = new Vector2Int(boardPosition.x - 1, boardPosition.y - 1);
            pieceAtPosition = GameController.GetFromBoard(toCheck);
            if (pieceAtPosition != null && pieceAtPosition.Color != this.Color && pieceAtPosition.PieceType == Constants.Pawn)
            {
                return true;
            }

        }
        //Finally, check for king 
        for (int i = -1; i <= 1; i++)
        {
            for (int k = -1; k <= 1; k++)
            {
                toCheck = new Vector2Int(boardPosition.x + i, boardPosition.y + k);
                Piece pieceToCheck = GameController.GetFromBoard(toCheck);
                if (CheckPieceNullOrKing(pieceToCheck))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool WithinBound(int value, int bound, int direction)
    {
        if (direction == -1 && value > bound)
        {
            return true;
        }
        if (direction == 1 && value < bound)
        {
            return true;
        }
        return false;
    }
    protected bool CheckPieceNullOrCheckingRQ(Piece pieceAtPosition)
    {
        if (pieceAtPosition != null && pieceAtPosition.Color != this.Color)
        {
            if (pieceAtPosition.PieceType == Constants.Queen || pieceAtPosition.PieceType == Constants.Rook)
            {
                return true;
            }
        }
        return false;
    }
    //For checking diagonals, bishop or queen
    protected bool CheckPieceNullOrCheckingBQ(Piece pieceAtPosition)
    {
        if (pieceAtPosition != null && pieceAtPosition.Color != this.Color)
        {
            if (pieceAtPosition.PieceType == Constants.Bishop || pieceAtPosition.PieceType == Constants.Queen)
            {
                return true;
            }
        }
        return false;
    }
    protected bool CheckPieceNullOrKing(Piece pieceAtPosition)
    {
        if (pieceAtPosition != null && pieceAtPosition.Color != this.Color)
        {
            if (pieceAtPosition.PieceType == Constants.King)
            {
                return true;
            }
        }
        return false;
    }
    public static bool InBounds(Vector2Int position)
    {
        if (position.x >= 0 && position.y >= 0 && position.x <= 7 && position.y <= 7)
        {
            return true;
        }
        return false;
    }
    
}