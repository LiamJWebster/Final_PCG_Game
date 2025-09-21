using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

    [SerializeField] private Transform Cam;
    [SerializeField] private Camera m_OrthographicCamera;

    [SerializeField] private Transform Grid_Manager;

    public enum levelObjects
    {
        playerSpawn,
        enemySpawn,
        stairway
    } 

    //Drawing the floor
    public void GenerateFloorExp()
    {
        int[,] floorCoord = new int[78, 24];
        int[,] roomDetails = new int[9, 5]; //x start / y start / x size / y size / type of room
        //2padding//6-24 room tiles//2padding//

        int[] floorArray = FloorInfo(Random.Range(5, 10));
        for (int i = 0; i < roomDetails.GetLength(0); i++)
        {
            roomDetails[i, 4] = floorArray[i];
        }

        //Creating the 9 floors in a square 
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                int Grid = (x * 3) + y;

                int xSize = Random.Range(6, 21 + 1);
                int ySize = Random.Range(3, 5 + 1);

                int xStart = Random.Range(1, 24 - xSize);
                int yStart = Random.Range(1, 7 - ySize);

                if (floorArray[Grid] > 0)
                {
                    roomDetails[Grid, 0] = xStart + (x * 26);
                    roomDetails[Grid, 1] = yStart + (y * 8);
                    roomDetails[Grid, 2] = xSize;
                    roomDetails[Grid, 3] = ySize;
                }
                else
                {
                    roomDetails[Grid, 0] = 0;
                    roomDetails[Grid, 1] = 0;
                    roomDetails[Grid, 2] = 0;
                    roomDetails[Grid, 3] = 0;
                }

                int[,] roomCoord = GenerateFloorSection(xSize, ySize, xStart, yStart, floorArray[Grid]);

                floorCoord = GenerateRoomExp(floorCoord, roomCoord, 26, 8, x, y);


            }
        }

        //connecting the floors
        // roomdetails floorarray
        floorCoord = roomConnections(floorCoord, roomDetails);
        floorCoord = spawnPlayer(floorCoord, roomDetails);

        //Generating the tiles / "drawing the floor"
        for (int x = 0; x < 78; x++)
        {
            for (int y = 0; y < 24; y++)
            {
                int value = floorCoord[x, y];
                //Grid_Manager
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(Grid_Manager, true);

                spawnedTile.Init(value, new Vector2(x, y));//This has been added due to changes in Tile structure in the testing enviorment if I hit a dead end remove

                if (value == 5)
                {
                    var Player = Instantiate(playerPrefab, new Vector3(x, y), Quaternion.identity);
                    Player.name = $"Player {x} {y}";
                }
                else if (value == 4)
                {
                    var spawnedEnemy = Instantiate(enemyPrefab, new Vector3(x, y), Quaternion.identity);
                    spawnedEnemy.name = $"Enemy {x} {y}";                    
                }
            }
        }

        //setting the camera into position
        Cam.transform.position = new Vector3((float)78 / 2 - 0.5f, (float)24 / 2 - 0.5f, -10);

        // Set the size of the viewing volume you'd like the orthographic Camera to pick up
        //m_OrthographicCamera.orthographicSize = (float)10;
        //GameManager.Instance.ChangeState(GameManager.GameState.SpawnHeroes);
    }

    //sets which areas on the grid rooms will generate 
    static int[] FloorInfo(int roomNum)
    {
        int roomNumber = roomNum;
        //Debug.Log("Number of rooms:" + roomNumber);

        //declare array and fill it
        int[] rooms = new int[9];
        for (int i = 0; i < 9; i++)
        {
            rooms[i] = 0;
        }

        while (0 < roomNumber)
        {
            int roomSelect = Random.Range(0, 9);
            if (rooms[roomSelect] == 0)
            {
                rooms[roomSelect] = 1;
                roomNumber--;
            }
        }

        return rooms;
    }

    static int[,] GenerateFloorSection(int xSize, int ySize, int xStart, int yStart, int generate)
    {
        //X 2padding//6-24 room tiles//2padding//
        //Y 1padding if on edge // 3-5 room tiles // 2 padding towards center
        //Y if in center row 1padding// 3-6 room tiles // 1 padding

        xSize = xStart + xSize;
        ySize = yStart + ySize;

        int[,] gridArray = new int[26, 8];

        for (int x = 0; x < 26; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if ((x >= xStart && x <= xSize) && (y >= yStart && y <= ySize) && (generate > 0))
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

    // Feeding the individual room areas to the floor
    static int[,] GenerateRoomExp(int[,] floor, int[,] newSection, int xSize, int ySize, int i, int j)
    {
        i = i * xSize;
        j = j * ySize;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                floor[x + i, y + j] = newSection[x, y];
            }
        }

        return floor;
    }
        
    //generating connections between rooms
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



        for (int j = 0; j < connections.GetLength(0); j++)
        {
            if ((connections[j, 0] > 0) && (connections[j, 2] != connections[j, 3]))
            {
                floor = drawConnections(floor, roomDetails, connections[j, 2], connections[j, 3]);
            }

        }

        return floor;
    }

    //testing function delete after turned into its own thing now don't delete re-evaluate probably rename
    static int[,] drawConnections(int[,] floor, int[,] roomDetails, int start, int end)
    {
        if ((int)Mathf.Floor(start / 3) == (int)Mathf.Floor(end / 3)) // vetrical
        {
            int startPos = 0;
            int endPos = 0;
            if (start > end)
            {
                startPos = end;
                endPos = start;
            }
            else
            {
                startPos = start;
                endPos = end;
            }

            int startY = roomDetails[startPos, 1] + roomDetails[startPos, 3] + 1; //added the distance - x size
            int startX = Random.Range(roomDetails[startPos, 0] + 1, roomDetails[startPos, 0] + roomDetails[startPos, 2] - 1);

            int endY = roomDetails[endPos, 1] - 1;//removed the distance - x size
            int endX = Random.Range(roomDetails[endPos, 0] + 1, roomDetails[endPos, 0] + roomDetails[endPos, 2] - 1);

            int inflection = Random.Range(startY+1, endY);

            if (endX > startX)
            {
                for (int i = 0; i <= endX - startX; i++)
                {
                    floor[startX + i, inflection] = 2;
                }
            }
            else
            {
                for (int i = 0; i <= startX - endX; i++)
                {
                    floor[startX - i, inflection] = 2;
                }

            }

            for (int i = 0; i <= inflection - startY; i++)
            {
                floor[startX, startY + i] = 2;
            }
            for (int i = 0; i <= endY - inflection; i++)
            {
                floor[endX, inflection + i] = 2;
            }

        }
        else // horizontal
        {
            int startPos = 0;
            int endPos = 0;
            if (start > end) // setting the lower x value as the first value
            {
                startPos = end;
                endPos = start;
            }
            else
            {
                startPos = start;
                endPos = end;
            }

            int startX = roomDetails[startPos, 0] + roomDetails[startPos, 2] + 1; //added the distance - x size
            int startY = Random.Range(roomDetails[startPos, 1] + 1, roomDetails[startPos, 1] + roomDetails[startPos, 3] - 1);// this needs to be fixed

            int endX = roomDetails[endPos, 0] - 1;//removed the distance - x size
            int endY = Random.Range(roomDetails[endPos, 1] + 1, roomDetails[startPos, 1] + roomDetails[endPos, 3] - 1);

            int inflection = Random.Range(startX, endX + 1);

            if (endY > startY)
            {
                for (int i = 0; i <= endY - startY; i++) //draw y  !! this needs to be fixed if end Y is lower than start y this simply doesn't work Mathf.Abs()
                {
                    floor[inflection, startY + i] = 2;
                }
            }
            else
            {
                for (int i = 0; i <= startY - endY; i++) //draw y  !! this needs to be fixed if end Y is lower than start y this simply doesn't work Mathf.Abs()
                {
                    floor[inflection, startY - i] = 2;
                }

            }

            for (int i = 0; i <= inflection - startX; i++)
            {
                floor[startX + i, startY] = 2;
            }
            for (int i = 0; i <= endX - inflection; i++)
            {
                floor[inflection + i, endY] = 2;
            }

        }

        return floor;
    }

    //Do first before spawning enemies / enemies should not spawn in the starting room
    //this isn't just for spawning the player anymore
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

        int spawnRoomX = (int)Mathf.Floor((spawnRoom)/3);
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

        for (int i = 0; i < potentialCount-1; i++) 
        {
            
            potentialRooms.Pop();
            
        }

        selectedRoom = potentialRooms.Pop();
        //Debug.Log("Final selection" + selectedRoom);

        floor = spawnObject(floor, selectedRoom, roomDetails, levelObjects.stairway);

        for(int i = 0;i < roomDetails.GetLength(0); i++)
        {
            if((i != spawnRoom) && (roomDetails[i,4]>0))
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
    

    static int[,] spawnObject(int[,] floor, int room, int[,] roomDetails , levelObjects toSpawn)
    {
        int x = 0;
        int y = 0;

        switch (toSpawn)
        {
            case levelObjects.playerSpawn:
                x = Random.Range(roomDetails[room, 0] + 1, roomDetails[room,0] + roomDetails[room,2]); //x start / y start / x size / y size / type of room
                y = Random.Range(roomDetails[room, 1] + 1, roomDetails[room, 1] + roomDetails[room, 3]);
                floor[x, y] = 5;
                break;
            case levelObjects.enemySpawn:
                x = Random.Range(roomDetails[room, 0] + 1, roomDetails[room, 0] + roomDetails[room, 2]);
                y = Random.Range(roomDetails[room, 1] + 1, roomDetails[room, 1] + roomDetails[room, 3]);
                
                floor[x, y] = 4;                

                break;
            case levelObjects.stairway:
                x = Random.Range(roomDetails[room, 0] + 1, roomDetails[room, 0] + roomDetails[room, 2]);
                y = Random.Range(roomDetails[room, 1] + 1, roomDetails[room, 1] + roomDetails[room, 3]);
                
                floor[x, y] = 3;                

                break;
        }


        return floor;

    }
   
    

    // Start is called before the first frame update
    void Start()
    {
       GenerateFloorExp(); 

    }

    // Update is called once per frame
    void Update()
    {

    }
}

