using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public GameObject tilePrefabs;

    const int size = 8;

    [SerializeField] private GameObject[,] tiles = new GameObject[size,size];

    List<GameObject> adjTileList =  new List<GameObject>();

    GameManager gameManager;

    public int whiteCount { get; private set; }
    public int blackCount { get; private set; }
    public int availCount { get; private set; }
    public int emptyCount { get; private set; }

    public UnityEvent countChanged;

    public enum direction
    {
        UP, 
        DOWN,
        RIGHT, 
        LEFT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT
    }

    public class PositionOnBoard
    {
        public int x,y;

        public PositionOnBoard(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    PositionOnBoard[] whiteStartingPos = new PositionOnBoard[2]
    {
        new PositionOnBoard(3,3),
        new PositionOnBoard(4,4)
    };

    PositionOnBoard[] blackStartingPos = new PositionOnBoard[2]
    {
        new PositionOnBoard(4,3),
        new PositionOnBoard(3,4)
    };

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.turnChanged.AddListener(onTurnChanged);
        SpawnTile();
        initGame();
        checkBoard();
        countBoard();
    }

    void SpawnTile()
    {
        for(int y = 0; y < size; y++)
        {
            for(int x = 0;x < size; x++)
            {
                Vector2 pos = gameObject.transform.position;
                pos += new Vector2(x, y);
                pos -= new Vector2((float)x * 1/16f, (float)y* 1/16f);
                GameObject newTile = Instantiate(tilePrefabs, pos , Quaternion.identity, gameObject.transform);
                Tile tile = newTile.GetComponent<Tile>();
                tile.stateChanged.AddListener(onTileStateChanged);
                tile.onClick.AddListener(flipAndSpawnTiles);
                tile.pos = new PositionOnBoard(x,y);
                tiles[y,x] = newTile;
            }
        }
    }

    void initGame()
    {
        foreach(PositionOnBoard pos in whiteStartingPos)
        {
            Tile tile = tiles[pos.y, pos.x].GetComponent<Tile>();
            tile.setState(TileState.White);
        }
        foreach (PositionOnBoard pos in blackStartingPos)
        {
            Tile tile = tiles[pos.y, pos.x].GetComponent<Tile>();
            tile.setState(TileState.Black);
        }
        //foreach (GameObject obj in adjTileList)
        //{
        //    obj.SetActive(false);
        //}
    }

    void onTileStateChanged(GameObject tileObj)
    {
        Tile tile = tileObj.GetComponent<Tile>();
        PositionOnBoard pos = tile.pos;
        if (tile.currState == TileState.UnAvail || tile.currState == TileState.Avail)
        {
            return;
        }
        Tile adjTile;
        if (pos.y < size - 1)
        {
            adjTile = tiles[pos.y+1,pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.y > 0)
        {
            adjTile = tiles[pos.y - 1, pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x < size - 1)
        {
            adjTile = tiles[pos.y, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x > 0)
        {
            adjTile = tiles[pos.y, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x > 0 && pos.y > 0)
        {
            adjTile = tiles[pos.y -1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x < size - 1 && pos.y > 0)
        {
            adjTile = tiles[pos.y - 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x > 0 && pos.y < size - 1)
        {
            adjTile = tiles[pos.y + 1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        if (pos.x < size - 1 && pos.y < size - 1)
        {
            adjTile = tiles[pos.y + 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileState.UnAvail && !adjTileList.Contains(adjTile.gameObject))
            {
                adjTileList.Add(adjTile.gameObject);
            }
        }
        adjTileList.Remove(tileObj);
    }
    void countBoard()
    {
        whiteCount = 0;
        blackCount = 0;
        emptyCount = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Tile tile = tiles[y, x].GetComponent<Tile>();
                if (tile.currState == TileState.White)
                {
                    whiteCount++;
                }
                else if (tile.currState == TileState.Black)
                {
                    blackCount++;
                }
                else
                {
                    emptyCount++;
                }
            }
        }
        countChanged.Invoke();
    }

    void checkBoard()
    {
        availCount = 0;
        foreach(GameObject obj in adjTileList)
        {
            Tile tile = obj.GetComponent<Tile>();

            bool isAvail = checkTile(tile);
            if (isAvail)
            {
                availCount++;
                tile.setState(TileState.Avail);
            }
            else
            {
                tile.setState(TileState.UnAvail);
            }
        }
    }

    bool checkTile(Tile tile)
    {
        bool isAvail =
        checkTileDirectional(tile, direction.UP) || checkTileDirectional(tile, direction.DOWN) ||
        checkTileDirectional(tile, direction.RIGHT) || checkTileDirectional(tile, direction.LEFT) ||
        checkTileDirectional(tile, direction.UP_RIGHT) || checkTileDirectional(tile, direction.UP_LEFT) ||
        checkTileDirectional(tile, direction.DOWN_RIGHT) || checkTileDirectional(tile, direction.DOWN_LEFT);

        return isAvail;
    }

    bool checkTileDirectional(Tile tile, direction dir, int depth = -1)
    {
        Tile adjTile;
        PositionOnBoard pos = tile.pos;
        bool isAvail = false;
        depth++;
        if (dir == direction.UP)
        {
            if (pos.y >= size - 1)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y + 1, pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;                
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.DOWN)
        {
            if (pos.y <= 0)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y - 1, pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.RIGHT)
        {
            if (pos.x >= size - 1)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.LEFT)
        {
            if (pos.x <= 0)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y , pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.UP_RIGHT)
        {
            if (pos.x >= size - 1 || pos.y >= size - 1)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y + 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.UP_LEFT)
        {
            if (pos.x <= 0 || pos.y >= size - 1)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y + 1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.DOWN_RIGHT)
        {
            if (pos.x >= size - 1 || pos.y <= 0)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y - 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }
        else if (dir == direction.DOWN_LEFT)
        {
            if (pos.x <= 0 || pos.y <= 0)
            {
                return isAvail;
            }
            adjTile = tiles[pos.y - 1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                isAvail = checkTileDirectional(adjTile, dir, depth);
            }
            else if (adjTile.currState == gameManager.currTurn && depth > 0)
            {
                isAvail = true;
            }
            else
            {
                return isAvail;
            }
        }

        return isAvail;
    }

    void flipAndSpawnTiles(PositionOnBoard pos)
    {
        Tile tile = tiles[pos.y,pos.x].GetComponent<Tile>();
        tile.setState(gameManager.currTurn);

        if (checkTileDirectional(tile, direction.UP))
        {
            flipTilesDirectional(pos,direction.UP);
        }
        if (checkTileDirectional(tile, direction.DOWN))
        {
            flipTilesDirectional(pos, direction.DOWN);
        }
        if (checkTileDirectional(tile, direction.RIGHT))
        {
            flipTilesDirectional(pos, direction.RIGHT);
        }
        if (checkTileDirectional(tile, direction.LEFT))
        {
            flipTilesDirectional(pos, direction.LEFT);
        }
        if (checkTileDirectional(tile, direction.UP_RIGHT))
        {
            flipTilesDirectional(pos, direction.UP_RIGHT);
        }
        if (checkTileDirectional(tile, direction.UP_LEFT))
        {
            flipTilesDirectional(pos, direction.UP_LEFT);
        }
        if (checkTileDirectional(tile, direction.DOWN_RIGHT))
        {
            flipTilesDirectional(pos, direction.DOWN_RIGHT);
        }
        if (checkTileDirectional(tile, direction.DOWN_LEFT))
        {
            flipTilesDirectional(pos, direction.DOWN_LEFT);
        }

        gameManager.changeTurn();
    }
    void flipTilesDirectional(PositionOnBoard pos,direction dir)
    {
        Tile adjTile;

        if (dir == direction.UP)
        {
            if (pos.y >= size - 1)
            {
                return;
            }
            adjTile = tiles[pos.y + 1, pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.DOWN)
        {
            if (pos.y <= 0)
            {
                return;
            }
            adjTile = tiles[pos.y - 1, pos.x].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.RIGHT)
        {
            if (pos.x >= size - 1)
            {
                return;
            }
            adjTile = tiles[pos.y, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.LEFT)
        {
            if (pos.x <= 0)
            {
                return;
            }
            adjTile = tiles[pos.y, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.UP_RIGHT)
        {
            if (pos.x >= size - 1 && pos.y >= size - 1)
            {
                return;
            }
            adjTile = tiles[pos.y + 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.UP_LEFT)
        {
            if (pos.x <= 0 && pos.y >= size - 1)
            {
                return;
            }
            adjTile = tiles[pos.y + 1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.DOWN_RIGHT)
        {
            if (pos.x >= size - 1 && pos.y <= 0)
            {
                return;
            }
            adjTile = tiles[pos.y - 1, pos.x + 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }
        else if (dir == direction.DOWN_LEFT)
        {
            if (pos.x <= 0 && pos.y <= 0)
            {
                return;
            }
            adjTile = tiles[pos.y - 1, pos.x - 1].GetComponent<Tile>();
            if (adjTile.currState == TileStateMethod.GetOppositeColor(gameManager.currTurn))
            {
                adjTile.flipTile();
                flipTilesDirectional(adjTile.pos, dir);
            }
            else
            {
                return;
            }
        }

        return;
    }

    void onTurnChanged()
    {
        checkBoard();
        countBoard();
        gameManager.validateTurn();
    }

}
