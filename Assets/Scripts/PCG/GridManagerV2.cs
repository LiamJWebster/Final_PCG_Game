using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManagerV2 : MonoBehaviour
{
    public static GridManagerV2 Instance;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;    

    [SerializeField] private Transform Grid_Manager;
    

    [Header("Camera")]
    [SerializeField] private Transform Cam;
    [SerializeField] private Camera m_OrthographicCamera;

    [Header("Tiles")]
    [SerializeField] private Tile _Floor;
    [SerializeField] private Tile _Wall;
    [SerializeField] private Tile _Water;
    //[SerializeField] private Tile Floor;

    [SerializeField] public Color _Highlight;
    [SerializeField] public Color _HighlightFriend;
    [SerializeField] public Color _HighlightEnemy;
    [SerializeField] public Color _HighlightNeutral;

    public int[,] tester;


    private Dictionary<Vector2, Tile> _tiles;

    [Header("Generation Settings")]
    [SerializeField] public int _width = 18;
    [SerializeField] public int _height = 14;
    [SerializeField] public int _minSize = 8;
    [SerializeField] public int _MaxX = 8;
    [SerializeField] public int _MaxY = 8;
    [SerializeField] public int _padding = 0;

    [Header("Pathfinding")]
    [SerializeField] private List<AStarNode> AStarPath;
    private bool atTarget = false;

    public enum levelObjects
    {
        playerSpawn,
        enemySpawn,
        stairway
    }

    public void GenerateTestingRoom(int width, int height , int minSize , int MaxX, int MaxY ,int padding = 2)
    {
        _tiles = new Dictionary<Vector2, Tile>();

        int roomType = UnityEngine.Random.Range(0, 3);

        int[,] floorCoord = new int[width, height];

        int xSize = UnityEngine.Random.Range(minSize, MaxX + 1);
        int ySize = UnityEngine.Random.Range(minSize, MaxY + 1);

        int xStart = UnityEngine.Random.Range(padding, width - xSize - padding + 1);
        int yStart = UnityEngine.Random.Range(padding, height - ySize - padding + 1);

        //chose room type
        if (roomType == 0)
        {
            floorCoord = GenerateRoom(xSize, ySize, xStart, yStart, width, height);
        }
        else if (roomType == 1)
        {
            floorCoord = FillRoom(floorCoord, true);
            floorCoord = CellularAutomata(floorCoord);
        }
        else if (roomType == 2) 
        {
            floorCoord = DrunkardsWalk(floorCoord);
        }

        ///////TESTING///////////
        /* debugging / video  
        floorCoord = GenerateRoom(xSize, ySize, xStart, yStart, width, height);        

        floorCoord = FillRoom(floorCoord, true);
        floorCoord = CellularAutomata(floorCoord);

        floorCoord = DrunkardsWalk(floorCoord);

        floorCoord[5,5] = 3;
        floorCoord[5, 6] = 3;
        floorCoord[7, 6] = 3;
        floorCoord[6, 6] = 3;

        floorCoord[7, 7] = 2;
        floorCoord[7, 8] = 2;
        floorCoord[7, 9] = 2;
        */
        ///////TESTING///////////

        int randRiver = UnityEngine.Random.Range(0,100);
        if(randRiver < 40)
        {
            floorCoord = GenerateRiver(floorCoord);
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int value = floorCoord[x, y];

                Tile toSpawn = SelectTileType(value); 
                
                var spawnedTile = Instantiate(toSpawn, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(Grid_Manager, true);

                spawnedTile.Init(value, new Vector2(x, y));

                _tiles[new Vector2 (x, y)] = spawnedTile;
            }
        }
        // to be moved to game manager later 
        tester = floorCoord;
        GameManager.Instance.ChangeState(GameManager.GameState.SpawnHeroes);

    }


    public void TestAutomata(int[,] floor)//used for debbuging 
    {
        _tiles = new Dictionary<Vector2, Tile>();


        int[,] floorCoord = floor;

        //floorCoord = FillRoom(floorCoord, true);
        floorCoord = CellularAutomata(floorCoord);

        for (int x = 0; x < floor.GetLength(0); x++)
        {
            for (int y = 0; y < floor.GetLength(1); y++)
            {
                int value = floorCoord[x, y];

                Tile toSpawn = SelectTileType(value);

                var spawnedTile = Instantiate(toSpawn, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(Grid_Manager, true);

                spawnedTile.Init(value, new Vector2(x, y));

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        // to be moved to game manager later 

        tester = floorCoord;
        GameManager.Instance.ChangeState(GameManager.GameState.SpawnHeroes);

    }

    private Tile SelectTileType(int value)
    {
        if (value == 1)
        {
            return _Floor;
        }
        else if (value == 2)
        {
            return _Water;
        }
        else
        {
            return _Wall;
        }

    }

    public int[,] GenerateRiver(int[,] floor)
    {
        int x = Mathf.FloorToInt(_width / 3);
        int y = 0;

        while(y < _height)
        {
            if (floor[x,y]!=0)
            {
                floor[x,y] = 2;
            }

            int rand = UnityEngine.Random.Range(0, 100);
            if(rand > 60)
            {
                x++;
                if (floor[x, y] != 0)
                {
                    floor[x, y] = 2;
                }
            }
            y++;
        }

        return floor;
    }

    public int[,] FillRoom(int[,] floor , bool Random)
    {
        for (int x = 0; x < floor.GetLength(0); x++)
        {
            for (int y = 0; y < floor.GetLength(1); y++) 
            {
                if (!Random)
                {
                    floor[x, y] = 0;
                }
                else
                {
                    floor[x,y] = UnityEngine.Random.Range(0, 2);
                }
                
            }
        }

        return floor;
    }
    

    public int[,] DrunkardsWalk(int [,] floor,int numDrunks = 10, int InputSteps = 25)
    {
        for (int i = 0; i < numDrunks; i++)
        {
            int numSteps = InputSteps;
            int startX = Mathf.FloorToInt(_width / 2);
            int startY = Mathf.FloorToInt(_height / 2);

            while (numSteps >= 0)
            {
                floor[startX, startY] = 1;
                int Direction = UnityEngine.Random.Range(1, 5);

                switch (Direction)
                {
                    case 1:
                        startX = (int)MathF.Min(startX + 1, _width-1);
                        break;
                    case 2:
                        startX = (int)MathF.Max(startX - 1, 0);
                        break;
                    case 3:
                        startY = (int)MathF.Min(startY + 1, _height-1);
                        break;
                    case 4:
                        startY = (int)MathF.Max(startY - 1, 0);
                        break;
                }

                numSteps--;
            }

        }
        return floor;
    }

    public int[,] CellularAutomata(int[,] grid, int Wall = 4, int floor = 4, int numCycles = 10)
    {    
        for (int i = 0; i < numCycles; i++)
        {
            int[,] Cells = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Cells[x, y] = grid[x, y];
                }
            }

            for (int x = 0; x < grid.GetLength(0); x++)//grid.GetLength(0)
            {
                for (int y = 0; y < grid.GetLength(1); y++)//grid.GetLength(1)
                {
                    int numFloor = 0;
                    int numWall = 0;
                    for (int x2 = x - 1; x2 < x + 2; x2++)
                    {
                        for (int y2 = y - 1; y2 < y + 2; y2++)
                        {
                            if (x2 >= _width || x2 < 0 || y2 < 0 || y2 >= _height)
                            {
                                numWall++;
                            }
                            else if(x2 != x || y2 != y)
                            {
                                if (grid[x2, y2] == 0)
                                {
                                    numWall++;
                                }
                                else
                                {
                                    numFloor++;
                                }
                            }
                        }
                    }
                    if (numFloor >= 4)
                    {
                        Cells[x,y] = 1;
                    }
                    else if(numWall > 4)
                    {
                        Cells[x, y] = 0;
                    }
                }                
            }
            grid = Cells;
            if (CheckConnectivity(grid))
            {
                return grid;
            }
        }
        bool ConnectivitySafe = false;
        while (ConnectivitySafe == false)
        {
            if (CheckConnectivity(grid))
            {
                return grid;
            }
            else
            {
                grid = DrunkardsWalk(grid);
            }

        }
        return grid;
    }

    public bool CheckConnectivity(int[,] grid)
    {
        bool[,] OverView = new bool[grid.GetLength(0), grid.GetLength(1)];

        bool firstFound = false;

        List<Vector2> toCheck = new List<Vector2>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                OverView[x, y] = false;
                if (!firstFound && grid[x,y] != 0)
                {
                    OverView[x, y] = false;
                    toCheck.Add(new Vector2(x, y));
                    firstFound = true;
                }                
            }
        }

        int counter = 0;
        while(toCheck.Count > 0)
        {
            counter++;
            if (counter >= 1500)
            {
                return false;
            }
            if (OverView[(int)toCheck[0].x, (int)toCheck[0].y] == false)
            {
                OverView[(int)toCheck[0].x, (int)toCheck[0].y] = true;
                Vector2[] nextTiles = getAdjacentTiles(toCheck[0]);

                for (int i = 0; i < nextTiles.Length; i++)
                {                    
                    int x = (int)nextTiles[i].x;
                    int y = (int)nextTiles[i].y;

                    if (grid[x, y] != 0)
                    {
                        toCheck.Add(nextTiles[i]);
                    }
                }
            }            
            toCheck.RemoveAt(0);
        }

        bool Connected = true;
        int gridSize = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {                
                if(OverView[x, y] == false && grid[x,y] != 0)
                {
                    Connected = false;
                }
                if(OverView[x, y] && grid[x, y] != 0)
                {
                    gridSize++;
                }
            }
        }

        if(gridSize <= 28) 
        {
            return false; // grid too small regen 
        }

        return Connected;
    }

    static int[,] GenerateRoom(int xSize, int ySize, int xStart, int yStart, int width ,int length)
    {
        xSize = xStart + xSize;
        ySize = yStart + ySize;

        int[,] gridArray = new int[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if ((x >= xStart && x <= xSize) && (y >= yStart && y <= ySize))
                {
                    gridArray[x, y] = 1;
                }
                else
                {
                    gridArray[x, y] = 0;
                }

            }
        }

        return gridArray;
    }

    public void Start()
    {
        Cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 1f, -10);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }
    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _width /2  && t.Value.IsClear).OrderBy(t => UnityEngine.Random.value).First().Value; /// 2
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width / 2  && t.Value.IsClear).OrderBy(t => UnityEngine.Random.value).First().Value; 
    }

    public Tile GetEnemySpawnTile(Vector2 pos)
    {
        List<Tile> list = new List<Tile>();

        for(int x = (int)pos.x - 2; x <  pos.x + 2; x++)
        {
            for(int y = (int)pos.y - 2; y < pos.y + 2; y++)
            {
                if(x >= 0 && y >= 0 && y < _height && x < _width)
                {
                    Tile CheckTile = GetTileAtPosition(new Vector2(x, y));
                    if (CheckTile.IsWalkable() && CheckTile.OccupiedUnit == null && CheckTile.enviromentalEffect == null) // this is a bit silly maybe write a function in tile to return if it is clear to be spawned in
                    {
                        list.Add(CheckTile);
                    }
                }
            }
        }
        if (list.Count > 0)
        {
            Debug.Log("Spawn ERROR");            
        }
        Tile tile = list[UnityEngine.Random.Range(0, list.Count)];
        return tile;
    }

    public Tile GetItemSpawn()
    {
        List<Tile> list = new List<Tile>();
        for(int x=0; x< _width; x++)
        {
            for(int y=0; y< _height; y++)
            {
                if (_tiles[new Vector2(x,y)].IsWalkable() && _tiles[new Vector2(x, y)].GetUnit() == null && _tiles[new Vector2(x, y)].GetItem() == null && _tiles[new Vector2(x, y)].GetObject() == null)
                {
                    list.Add(_tiles[new Vector2(x, y)]);
                }
            }
        }        

        Tile ItemTile = list[UnityEngine.Random.Range(0, list.Count)];
        return ItemTile;
    }

    public List<AStarNode> GetPath()
    {
        return AStarPath;
    }

    public void MovementHighlight(Vector2 pos , int movementpoints)
    {
        // make more effeceint change the search pattern to a smaller area of tiles based on movmeent input
        int startY = (int)Mathf.Max(pos.y - movementpoints,0);
        int startX = (int)Mathf.Max(pos.x - movementpoints,0);

        int endX = (int)Mathf.Min((int)pos.x + movementpoints + 1, _width);
        int endY = (int)Mathf.Min((int)pos.y + movementpoints + 1, _height);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {                
                int distance = Mathf.Abs((int)pos.y - y) + Mathf.Abs((int)pos.x - x);

                if((distance <= movementpoints) && _tiles[new Vector2(x, y)].IsWalkable())
                {
                    if(_tiles[new Vector2(x, y)].GetUnit() != null && (pos != new Vector2(x,y)))
                    {
                        switch (_tiles[new Vector2(x, y)].GetUnit().Faction)
                        {
                            case Faction.Hero:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightFriend);
                                break;
                            case Faction.Enemy:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightEnemy);
                                break;
                            case Faction.Neutral:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightNeutral);
                                break;
                        }
                    }                     
                    _tiles[new Vector2(x, y)].ActivateHighlight();
                }                

            }
        }      

     }
    
    public void Djikstra(Vector2 pos, int movementpoints, int[] tileWeights)
    {
        int endX = _width;
        int endY = _height;

        bool[,] toDraw = new bool[_width, _height];
        int[,] RouteDist = new int[_width, _height];

        for(int i = 0; i < _width; i++)
        {
            for(int j = 0; j < _height; j++)
            {
                toDraw[i, j] = false;
            }
        }

        List<Vector2> toCheck = new List<Vector2>();
        List<int> remainingMovement = new List<int>();

        Vector2[] startingTiles = getAdjacentTiles(pos);
        toDraw[(int)pos.x, (int)pos.y] = true;
        RouteDist[(int)pos.x, (int)pos.y] = 0;

        toCheck.Add(pos);
        remainingMovement.Add(movementpoints);
        
        while (toCheck.Count > 0)
        {
            if (remainingMovement[0] > 0) //if there are movement points left we keep expanding the grid
            {
                Vector2[] nextTiles = getAdjacentTiles(toCheck[0]);
                for (int i = 0; i < nextTiles.Length; i++)
                {
                    int remainingMP;
                    int x = (int)nextTiles[i].x;
                    int y = (int)nextTiles[i].y;

                    //clean up and improve
                    Tile.tileType SelectedTile = GetTileAtPosition(nextTiles[i])._tileType;
                    int movementCost;
                    switch (SelectedTile)
                    {
                        case Tile.tileType.floor:
                            movementCost = 1;
                            break;
                        case Tile.tileType.water:
                            movementCost = 2;
                            break;
                        case Tile.tileType.wall:
                            movementCost = 99;
                            break;
                        default:
                            Debug.Log("WEEWOOWEEEWOOO");//just for debugging purposes should never hit default ideally
                            movementCost = 1;
                            break;
                    }

                    if(GetTileAtPosition(nextTiles[i]).ObjectWalkable() == false)
                    {
                        movementCost = 99;
                    }
                    remainingMP = remainingMovement[0] - movementCost;

                    if (remainingMP >= 0)
                    {
                        // remove above after debugging
                        if (toDraw[x, y] == false)
                        {
                            toCheck.Add(nextTiles[i]);
                            remainingMovement.Add(remainingMP);
                            toDraw[x, y] = true;
                            RouteDist[x, y] = movementpoints - remainingMP;
                        }
                        else if (RouteDist[x, y] < remainingMP)
                        {
                            toCheck.Add(nextTiles[i]);
                            remainingMovement.Add(remainingMP);
                            toDraw[x, y] = true;
                            RouteDist[x, y] = movementpoints - remainingMP;
                        }
                    }
                    //clean up and improve//////////////////
                }
            }
            toCheck.RemoveAt(0);
            remainingMovement.RemoveAt(0);    
        }
        
        for (int x = 0; x < endX; x++)
        {
            for (int y = 0; y < endY; y++)
            {//for statements are fine
                // && _tiles[new Vector2(x, y)].IsWalkable()
                if (toDraw[x, y])
                {
                    if (_tiles[new Vector2(x, y)].GetUnit() != null && (pos != new Vector2(x, y)))
                    {
                        switch (_tiles[new Vector2(x, y)].GetUnit().Faction)
                        {
                            case Faction.Hero:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightFriend);
                                break;
                            case Faction.Enemy:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightEnemy);
                                break;
                            case Faction.Neutral:
                                _tiles[new Vector2(x, y)].ActivateHighlight(_HighlightNeutral);
                                break;
                        }
                    }
                    _tiles[new Vector2(x, y)].ActivateHighlight();
                }

            }
        }

    }

    public class AStarNode
    {
        public AStarNode previousNode;
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;

        public Vector2 position { get; set; }

        public void SetConnection(AStarNode node)
        {
            previousNode = node;
        }

    }

    public void AStar(Vector2 Startpos, int movementpoints, int[] tileWeights, Vector2 EndPos, Faction unitFaction , bool SecondCall = false)
    {        
        if ((unitFaction == Faction.Enemy || unitFaction == Faction.Neutral) && Vector2.Distance(EndPos, Startpos) <= 1)
        {
            atTarget = true;
        }

        List<Vector2> Checked = new List<Vector2>();
        bool solutionFound = false;

        List<AStarNode> Nodes = new List<AStarNode>();

        AStarNode StartNode = new AStarNode();
        StartNode.G = 0;
        StartNode.H = (int)Mathf.Abs(EndPos.x - Startpos.x) + (int)Mathf.Abs(EndPos.y - Startpos.y);
        StartNode.position = Startpos;
        Nodes.Add(StartNode);

        int pos = 0;
        List<AStarNode> path = new List<AStarNode>(); // the final list that will be used to highlight tiles

        int Ecount = 0;
        while (solutionFound == false)
        {
            Ecount++;
            if (Ecount > 300)
            {
                //AI
                if (unitFaction == Faction.Enemy || unitFaction == Faction.Neutral && SecondCall)
                {
                    if (SecondCall)
                    {
                        Debug.Log("Second Call Error");
                        break;
                    }
                    //change target if can't reach the selected target
                    int target = UnitManager.Instance.GetCurrentEnemy().Target;
                    while(UnitManager.Instance.GetCurrentEnemy().Target == target)
                    {
                        UnitManager.Instance.SetEnemyTarget(UnitManager.Instance.currentEnemy);
                    }
                    AStar(Startpos, movementpoints, tileWeights, EndPos, unitFaction, true);
                }
                //Debug.Log("Pathfinder Error");
                break;
            }
            AStarNode closestNode = null;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (closestNode == null || ((closestNode.F == Nodes[i].F) && (closestNode.H > Nodes[i].H)) || closestNode.F > Nodes[i].F)
                {
                    closestNode = Nodes[i];
                    pos = i;
                }
            }
            //Check if the selected tile is the final tile
            if (closestNode.position == EndPos)
            {
                var PathNode = closestNode;
                var count = 200;
                while (PathNode.position != Startpos)
                {
                    path.Add(PathNode);
                    PathNode = PathNode.previousNode;
                    count--;
                    if (count < 0)
                    {
                        Debug.Log("sdfsdf");
                        throw new Exception();
                    }
                }
                break;
            }

            if(unitFaction == Faction.Enemy || unitFaction == Faction.Neutral)
            {
                //int x = 0;
                //int y = 0;
                //int dist = 0; 
                if(Vector2.Distance(EndPos,closestNode.position) <= 1)
                {
                    var PathNode = closestNode;
                    var count = 200;
                    while (PathNode.position != Startpos)
                    {
                        path.Add(PathNode);
                        PathNode = PathNode.previousNode;
                        count--;
                        if (count < 0)
                        {
                            Debug.Log("sdfsdf");
                            throw new Exception();
                        }
                    }
                    break;
                }
            }

            Vector2[] nextTiles = getAdjacentTiles(Nodes[pos].position);
            int newG = closestNode.G;
            for (int i = 0; i < nextTiles.Length; i++)
            {
                bool AlreadyChecked = false;
                for (int j = 0; j < Checked.Count(); j++)
                {
                    if (nextTiles[i] == Checked[j])
                    {
                        AlreadyChecked = true;
                    }
                }
                // I think the below might need to be changed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (!AlreadyChecked)
                {
                    Tile checkTile = GetTileAtPosition(nextTiles[i]);
                    Tile.tileType SelectedTile = GetTileAtPosition(nextTiles[i])._tileType;
                    int movementCost;
                    switch (SelectedTile)
                    {
                        case Tile.tileType.floor:
                            movementCost = 1;
                            break;
                        case Tile.tileType.water:
                            movementCost = 2;
                            break;
                        case Tile.tileType.wall:
                            movementCost = 99;
                            break;
                        default:
                            movementCost = 1;
                            break;
                    }

                    newG = closestNode.G + movementCost;

                    AStarNode newNode;
                    newNode = new AStarNode();
                    bool enemyPresent = false;

                    if (checkTile.OccupiedUnit != null)
                    {
                        if (checkTile.OccupiedUnit.Faction != unitFaction || Vector2.Distance(Nodes[pos].position, checkTile._Pos) < 1)// dealing with enemies piling up
                        {
                            enemyPresent = true;
                        }
                    }
                    if ((checkTile.IsWalkable() == false) || enemyPresent == true || checkTile.ObjectWalkable() == false)
                    {
                        newNode.G = 999; // setting tiles that are unwalkable to be unreachable
                    }
                    else
                    {
                        newNode.G = newG;
                    }
                    newNode.H = (int)Mathf.Abs(EndPos.x - nextTiles[i].x) + (int)Mathf.Abs(EndPos.y - nextTiles[i].y);
                    newNode.position = nextTiles[i];
                    newNode.previousNode = closestNode;
                    Nodes.Add(newNode);

                }
                
            }
            Nodes.RemoveAt(pos);
        }
        AStarPath = path;
    }
         
    public void DrawAstar(int heroMovement = 5)
    {
        if (heroMovement <= 0)
        {
            return;
        }
        int remainingMP = heroMovement;
        List<AStarNode> path = new List<AStarNode> ();

        for(int i = 0; i < AStarPath.Count; i++)
        {
            path.Add(AStarPath[i]);
        }

        bool endFound = false;
        if(path.Count == 0)
        {
            endFound = true;
        }

        while (endFound == false)
        {
            var toDraw = path[path.Count - 1];
            path.RemoveAt(path.Count - 1);
            
            
            if (_tiles[toDraw.position].GetUnit() != null)
            {
                _tiles[toDraw.position].ActivateHighlight(_HighlightFriend);
            }
            _tiles[toDraw.position].ActivateMovement();

            switch (_tiles[toDraw.position]._tileType)
            {
                case Tile.tileType.floor:
                    remainingMP--;
                    break;
                case Tile.tileType.water:
                    remainingMP -= 2;
                    break;
                default:
                    break;

            }

            if (remainingMP <= 0 || path.Count < 1)
            {
                endFound = true;
            }
        }
    }

    public void AstarMove(BaseUnit Unit, int UnitMovement)
    {
        if (atTarget)
        {
            atTarget = false;
            return;
        }
        if (UnitMovement <= 0)
        {
            return;
        }
        int remainingMP = UnitMovement;
        List<AStarNode> path = new List<AStarNode>();

        for (int i = 0; i < AStarPath.Count; i++)
        {
            path.Add(AStarPath[i]);
        }

        bool endFound = false;
        
        int lastTileCost = 0;
        bool lastTileContainsUnit = false;  

        while (endFound == false)
        {
            if(path.Count <= 0) {break;}
            var toDraw = path[path.Count - 1];
            lastTileContainsUnit = false;

            if (_tiles[toDraw.position].GetUnit() != null)
            {
                lastTileContainsUnit = true;
            }

            switch (_tiles[toDraw.position]._tileType)
            {
                case Tile.tileType.floor:
                    lastTileCost = 1;
                    remainingMP -= lastTileCost;
                    break;
                case Tile.tileType.water:
                    lastTileCost = 2;
                    remainingMP -= lastTileCost;
                    break;
                default:
                    lastTileCost = 0;
                    break;

            }
            path.RemoveAt(path.Count - 1);

            if (remainingMP < 1 || path.Count < 1)
            {
                if (lastTileContainsUnit == true)
                {
                    AstarMove(Unit, UnitMovement - lastTileCost);
                    endFound = true;
                }
                else
                {
                    Unit.OccupiedTile.OccupiedUnit = null;
                    _tiles[toDraw.position].SetUnit(Unit);
                    Unit._MovementPoints = remainingMP;
                    if(Unit.Faction == Faction.Hero)
                    {
                        UnitManager.Instance.selectedHero = null;
                    }
                    CleanMovementHighlight();
                    endFound = true;
                }
            }
        }        
    }

    public void EraseAstar()
    {
        if (AStarPath != null) 
        {
            var path = AStarPath;
            while (path.Count > 0)
            {
                var toDraw = path[0];
                {
                    _tiles[toDraw.position].DeactivateHighlight();
                    _tiles[toDraw.position].DeactivateMovement();
                }
                path.RemoveAt(0);
            }
        }
        
    }



    public Vector2[] getAdjacentTiles(Vector2 currentTile)
    {
        List<Vector2> Adjacent = new List<Vector2>();

        int x = (int)currentTile.x;
        int y = (int)currentTile.y;

        if (x-1 >= 0)
        {
            Adjacent.Add(new Vector2(x - 1, y));
        }
        if (x + 1 <= _width-1) 
        { 
            Adjacent.Add(new Vector2(x + 1, y));
        }
        if (y - 1 >= 0)
        {
            Adjacent.Add(new Vector2(x, y - 1));
        }
        if (y + 1 <= _height-1)
        {
            Adjacent.Add(new Vector2(x, y + 1));
        }

        return Adjacent.ToArray();
    }

    public void CleanMovementHighlight()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //if ((Vector2.Distance(pos, new Vector2(x, y)) <= movementpoints) && _tiles[new Vector2(x, y)].IsWalkable())
                //{
                    
                //}
                if(_tiles[new Vector2(x, y)].IsWalkable())
                {
                    _tiles[new Vector2(x, y)].DeactivateHighlight(_Highlight);
                    _tiles[new Vector2(x, y)].DeactivateMovement();
                }

            }
        }

    }

    //A* algo
    public void SelectMovementHighlight(Vector2 Unitpos, int movementpoints, Vector2 SelectedTile) 
    {

    }

    public void HighlightAttack(List<Vector2> tiles) 
    {
        for (int i =0; i < tiles.Count(); i++)
        {
            if (_tiles[tiles[i]].OccupiedUnit == null)
            {
                _tiles[tiles[i]].ActivateHighlight(_HighlightEnemy);
            }
            else
            {
                if(_tiles[tiles[i]].OccupiedUnit.Faction == Faction.Hero)
                {
                    _tiles[tiles[i]].ActivateHighlight(_HighlightFriend);
                }
                else
                {
                    _tiles[tiles[i]].ActivateHighlight(_HighlightEnemy);
                }                
            }

        }
    }

}
