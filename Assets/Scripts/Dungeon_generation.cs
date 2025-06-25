using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_generation : MonoBehaviour
{
    [SerializeField] private int NumberFloors;
    [SerializeField] private int minFloors;
    [SerializeField] private int maxFloors;
    [SerializeField] private int height;

    [SerializeField] private circle_floor circlePrefab;

    void GenerateDungeon()
    {
        int x = 0;
        int[] floorSizes = new int[8];
        int[] floorConnectionSize = new int[3];

        
        int holder = 3;
        floorSizes[0] = 1;
        floorSizes[1] = 3;
        floorSizes[6] = 3;
        floorSizes[7] = 1;
        for (int i = 2; i < 6; i++)
        {
            if (holder == 3)
            {
                floorSizes[i] = Random.Range(holder, holder + 2);

            }
            else if (holder == 4)
            {
                floorSizes[i] = Random.Range(holder - 1, holder + 2);

            }
            else if (holder == 5)
            {
                floorSizes[i] = Random.Range(holder - 1, holder + 1);

            }
            /*
            else
            {
                floorSizes[i] = Random.Range(holder - 1, holder + 1);
            }
            */

                holder = floorSizes[i];
        }

        
        for (int i = 0; i < 8; i++)
        {
                generateNode(floorSizes[i],i*2);           
            
        }

    }

    void generateNode(int numNodes, int x)
    {
        int yPos = (int)Mathf.Floor(numNodes / 2);
        int yStart = -3 * yPos;
        if (numNodes % 2 == 0)
        {
            float evenY = (float)yStart;
            evenY = evenY + 1.5f;
            for (int y = 0; y < numNodes; y++)
            {
                evenY += 3;


                var spawnedTile = Instantiate(circlePrefab, new Vector3(x, evenY), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {evenY}";

                int value = Random.Range(0, 4);
                spawnedTile.Init(value);
            }
        }
        else
        {
            for (int y = 0; y < numNodes; y++)
            {
                yStart += 3;

                var spawnedTile = Instantiate(circlePrefab, new Vector3(x, yStart), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {yStart}";

                int value = Random.Range(0, 4);
                spawnedTile.Init(value);
            }

        }
            

    }




    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
