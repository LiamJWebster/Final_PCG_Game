using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManagerV2 : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private Player playerPrefab;
    [SerializeField] private Enemy enemyPrefab;

    [SerializeField] private Transform Cam;
    [SerializeField] private Camera m_OrthographicCamera;

    [SerializeField] private Transform Grid_Manager;


    [Header("Generation Settings")]
    [SerializeField] private int _width = 12;
    [SerializeField] private int _height = 12;
    [SerializeField] private int _minSize = 8;
    [SerializeField] private int _MaxX = 8;
    [SerializeField] private int _MaxY = 8;
    [SerializeField] private int _padding = 0;

    public enum levelObjects
    {
        playerSpawn,
        enemySpawn,
        stairway
    }

    public void GenerateTestingRoom(int width, int height , int minSize , int MaxX, int MaxY ,int padding = 2)
    {
        minSize = minSize;
        MaxX = MaxX;
        MaxY = MaxY;

        int[,] floorCoord = new int[width, height];

        int xSize = Random.Range(minSize, MaxX + 1);
        int ySize = Random.Range(minSize, MaxY + 1);

        int xStart = Random.Range(padding, width - xSize - padding + 1);
        int yStart = Random.Range(padding, height - ySize - padding + 1);

        Debug.Log("Xstart " + xStart);
        Debug.Log("Ystart " + yStart);

        floorCoord = GenerateRoom(xSize, ySize, xStart, yStart, width, height);

        floorCoord[5,5] = 5;
        floorCoord[6, 6] = 4;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int value = floorCoord[x, y];
                //Grid_Manager
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(Grid_Manager, true);

                spawnedTile.Init(value);

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
        // to be moved to game manager later 
        

    }

    //Drawing the floor
    /*
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

                spawnedTile.Init(value);

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
    }*/

 

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
        GenerateTestingRoom(_width, _height, _minSize, _MaxX, _MaxY);
        Debug.Log("YO!");
        Cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }


}
