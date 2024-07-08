using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    //Will eventually contain player classes that contain pieces for each player
    public Player WhitePlayer;
    public Player BlackPlayer;
    public PieceFactory PieceFactory;
    public GameController GameController;
    public Grid Grid;
    public Tilemap Board;
    //Assigned in start function and represent edges of board in local space (child of grid)

    // Start is called before the first frame update
    void Start()
    {
        GameController = new GameController(PieceFactory);
        //Must occur in order
        InitalizePlayers();
        



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitalizePlayers()
    {
        WhitePlayer = new Player(PieceFactory, Constants.White, GameController);
        BlackPlayer = new Player(PieceFactory, Constants.Black, GameController);

    }
    
}
