using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private BaseFloorTile Floor;
    [SerializeField] private BaseFloorTile Wall;

    [SerializeField] private Transform Cam;
    [SerializeField] private Camera m_OrthographicCamera;
    [SerializeField] private Transform Grid_Manager;

    private Dictionary<Vector2, BaseFloorTile> _tiles;
    private RoomDetails[,] RoomDetailStorage;

    public class RoomDetails
    {
        public Vector2 startPos;
        public int roomWidth;
        public int roomHeight;

        public bool explored;
        public bool toSpawn;

        public List<RoomDetails> ConnectedRooms;
        public List<BaseFloorTile> FloorTiles; //store the tiles belonging to the room
        public List<BaseFloorTile> WallsTiles; //rename to connection tile
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

    private void Start()
    {
        GenerateFloor();
    }

    public void GenerateFloor(int width = 72, int height = 27, int RoomSize = 3, int numRows = 3, int numColumns = 3, int padding = 2)
    {
        _tiles = new Dictionary<Vector2, BaseFloorTile>();
        RoomDetailStorage = new RoomDetails[numRows, numColumns];

        //0 = wall
        //1 = floor

        int[,] floorCoord = new int[width, height];
        int[,] rooms = new int[numRows, numColumns];

        bool[,] GenRooms = RoomsToGen(numRows, numColumns);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                floorCoord[x, y] = 0;
            }
        }

        RoomDetailStorage = GenerateRooms(width, height, numRows, numColumns, RoomSize, padding, GenRooms);
        floorCoord = PrintRooms(RoomDetailStorage, floorCoord);

        // int xSize = Random.Range(minSize, MaxX + 1);
        //int ySize = Random.Range(minSize, MaxY + 1);

        //int xStart = Random.Range(padding, width - xSize - padding + 1);
        //int yStart = Random.Range(padding, height - ySize - padding + 1);

        //floorCoord = GenerateRooms(xSize, ySize, xStart, yStart, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int value = floorCoord[x, y];
                BaseFloorTile toSpawn;

                if (value == 0)
                {
                    toSpawn = Wall;
                }
                else
                {
                    toSpawn = Floor;
                }

                var spawnedTile = Instantiate(toSpawn, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(Grid_Manager, true);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

    }

    private bool[,] RoomsToGen(int Rows, int Columns)
    {
        bool[,] Rooms = new bool[Rows, Columns];
        for (int x = 0; x < Rows; x++)
        {
            for (int y = 0; y < Columns; y++)
            {
                Rooms[x, y] = false;
            }
        }
        int maxRooms = Rows * Columns;
        int numRooms = Random.Range(Mathf.FloorToInt(maxRooms / 2), maxRooms);

        while (numRooms > 0)
        {
            int randY = Random.Range(0, Rows);
            int randX = Random.Range(0, Columns);

            if (Rooms[randX, randY] == false)
            {
                Rooms[randX, randY] = true;
                numRooms--;
            }
        }

        return Rooms;
    }

    private RoomDetails[,] GenerateRooms(int width, int height, int Rows, int Columns, int roomSize, int padding, bool[,] roomSpawns)
    {
        int roomWidth = Mathf.FloorToInt(width / Rows);
        int roomHeight = Mathf.FloorToInt(height / Columns);
        RoomDetails[,] Rooms = new RoomDetails[Rows, Columns];

        int AdjustedPadding = padding + roomSize;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                RoomDetails newRoom = new RoomDetails();

                int startX = Random.Range((roomWidth * i) + padding, (roomWidth * (i + 1)) - AdjustedPadding);
                int startY = Random.Range((roomWidth * j) + padding, (roomWidth * (j + 1)) - AdjustedPadding);
                newRoom.startPos = new Vector2(startX, startY);

                newRoom.roomWidth = roomSize;
                newRoom.roomHeight = roomSize;

                newRoom.explored = false;
                newRoom.toSpawn = roomSpawns[i, j];

                newRoom.ConnectedRooms = new List<RoomDetails>();
                newRoom.FloorTiles = new List<BaseFloorTile>();
                newRoom.WallsTiles = new List<BaseFloorTile>();

            }
        }

        return Rooms;
    }

    private int[,] PrintRooms(RoomDetails[,] rooms, int[,] Grid)
    {
        for(int i = 0; i < rooms.GetLength(0); i++)
        {
            for(int j = 0; j < rooms.GetLength(1); j++)
            {
                if(rooms[i, j].toSpawn)
                {
                    //for (int x = 0; )
                   // {

                   // }
                } 
            }
        }
        return Grid;

    }
}

   
    /*
    static int[,] spawnPlayer(int[,] floor, int[,] roomDetails)
    {
        bool spawnSet = false;
        int spawnRoom = 0;

        while (!spawnSet)
        {
            int randomRoom = Random.Range(0, roomDetails.GetLength(0));

            if (roomDetails[randomRoom, 4] == 1)
            {
                spawnRoom = randomRoom;
                spawnSet = true;
            }
        }

        floor = spawnObject(floor, spawnRoom, roomDetails, levelObjects.playerSpawn);

        int spawnRoomX = (int)Mathf.Floor((spawnRoom) / 3);
        int spawnRoomY = spawnRoom - (spawnRoomX * 3);

        int highDist = 0;
        Stack<int> potentialRooms = new Stack<int>();

        for (int x = 0; x < 3; x++)//generating a stack containing the room or rooms that are fruthest away 
        {
            for (int y = 0; y < 3; y++)
            {
                int room = (x * 3 + y);
                if (roomDetails[room, 4] > 0)
                {
                    int mDist = Mathf.Abs((x - spawnRoomX) + (y - spawnRoomY)); //manhattan dist 
                    if (((x - spawnRoomX) != 0) && ((y - spawnRoomY) != 0)) // adding weight to diagonal
                    {
                        mDist = mDist + 1;
                    }

                    if (highDist < mDist)
                    {
                        highDist = mDist;
                        potentialRooms.Clear();
                        potentialRooms.Push(room);
                    }
                    else if (highDist == mDist)
                    {
                        potentialRooms.Push(room);
                    }
                }
            }
        }

        int potentialCount = Random.Range(0, potentialRooms.Count + 1);
        //Debug.Log("Number of rooms" + potentialRooms.Count);
        int selectedRoom = 0;

        for (int i = 0; i < potentialCount - 1; i++)
        {

            potentialRooms.Pop();

        }

        selectedRoom = potentialRooms.Pop();
        //Debug.Log("Final selection" + selectedRoom);

        floor = spawnObject(floor, selectedRoom, roomDetails, levelObjects.stairway);

        for (int i = 0; i < roomDetails.GetLength(0); i++)
        {
            if ((i != spawnRoom) && (roomDetails[i, 4] > 0))
            {
                int numEnemies = Random.Range(1, 4);// fix the code enemies can spawn on top of each other
                for (int j = 0; j < numEnemies; j++)
                {
                    floor = spawnObject(floor, i, roomDetails, levelObjects.enemySpawn);
                }
            }
        }

        return floor;
    }


    static int[,] roomConnections(int[,] floor, int[,] roomDetails)
    {
        //generate edges
        int[,] connections = new int[9, 4];
        for (int i = 0; i < connections.GetLength(0); i++)// fill the array
        {
            for (int j = 0; j < connections.GetLength(1); j++)
            {
                connections[i, j] = 0;
            }
        }

        Stack<int> edgeDist = new Stack<int>();
        Stack<int> edgeStart = new Stack<int>();
        Stack<int> edgeEnd = new Stack<int>();

        bool[,] gridInfo = new bool[3, 3]; // creating an array to store where rooms are and aren't
        int numRooms = 0; //storing the number of rooms
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int room = (x * 3 + y);
                if (roomDetails[room, 4] > 0)//check if room is generating maybe add logic
                {
                    gridInfo[x, y] = true;
                    numRooms++;
                }
                else
                {
                    gridInfo[x, y] = false;
                }
            }
        }

        for (int x1 = 0; x1 < 3; x1++)//generating edges
        {
            for (int y1 = 0; y1 < 3; y1++)
            {
                for (int x2 = 0; x2 < 3; x2++) //distance checks
                {
                    for (int y2 = 0; y2 < 3; y2++)
                    {
                        int mDist = Mathf.Abs((x2 - x1) + (y2 - y1)); //manhattan dist need top fix later diagonal should have xtra weight / distance
                        if (((x2 - x1) != 0) && ((y2 - y1) != 0))
                        {
                            mDist = mDist + 2;
                        }
                        int room1 = (x1 * 3 + y1);
                        int room2 = (x2 * 3 + y2);
                        if (((0 < mDist) && (mDist < 4)))
                        {
                            if (roomDetails[room2, 4] >= 1)
                            {
                                edgeDist.Push(mDist);
                                edgeStart.Push(room1);
                                edgeEnd.Push(room2);
                            }
                        }

                    }
                }
            }
        }

        int selectNode = 0;
        bool startFound = false;

        while (!startFound)//setting a random start node
        {
            int randomRoom = Random.Range(0, 8);
            if (roomDetails[randomRoom, 4] > 0)
            {
                selectNode = randomRoom;
                startFound = true;
            }
        }

        int[] distArray = edgeDist.ToArray();
        int[] startArray = edgeStart.ToArray();
        int[] endArray = edgeEnd.ToArray();

        edgeDist.Clear();
        edgeStart.Clear();
        edgeEnd.Clear();

        int[] usedEdges = new int[9];
        //Debug.Log("Selected node" + selectNode);

        for (int i = 0; i < usedEdges.Length; i++)
        {
            usedEdges[i] = -1;
        }


        for (int i = 0; i < numRooms; i++)
        {
            //Debug.Log("Selected node " + selectNode);
            usedEdges[i] = selectNode;

            Stack<int> PrimDist = new Stack<int>();
            Stack<int> PrimStart = new Stack<int>();
            Stack<int> PrimEnd = new Stack<int>();

            for (int k = 0; k < usedEdges.Length; k++)// cycles through all the starting // nodes to get their connections 
            {
                for (int j = 0; j < distArray.Length; j++) // cycles through the array to find all connections
                {

                    if (usedEdges[k] == startArray[j]) //Once a node matches with one of the selected nodes we check below to make sure it doesn't connect to one of the starting nodes
                    {
                        bool checkVisited = false;
                        for (int l = 0; l < usedEdges.Length; l++)
                        {
                            if (usedEdges[l] == endArray[j])// so I want to select edges starting at set indexes BUT i do not want to select edges ending at nodes I have already been to
                            {
                                checkVisited = true;
                            }
                        }

                        if (!checkVisited)
                        {
                            PrimDist.Push(distArray[j]);
                            PrimStart.Push(startArray[j]);
                            PrimEnd.Push(endArray[j]);
                        }
                    }
                }
            }
            int tempDistance = 5; //aribritrary number 5 can be changed
            int tempStart = 0;
            int tempEnd = 0;

            while (PrimDist.Count > 0)
            {
                if (PrimDist.Peek() < tempDistance)
                {
                    tempDistance = PrimDist.Pop();
                    tempStart = PrimStart.Pop();
                    tempEnd = PrimEnd.Pop();
                }
                else
                {
                    PrimDist.Pop();
                    PrimStart.Pop();
                    PrimEnd.Pop();
                }
            }

            connections[i, 0] = 1;
            connections[i, 1] = tempDistance;
            connections[i, 2] = tempStart;
            connections[i, 3] = tempEnd;

            selectNode = tempEnd; // think this should work

        }


    }*/


   
