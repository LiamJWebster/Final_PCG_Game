using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColour;
    [SerializeField] private Color WallColour;
    [SerializeField] private Color ConnectColour;
    [SerializeField] private Color StairsColour;

    [SerializeField] private Color enemySpawn;
    [SerializeField] private Color PlayerSpawn;

    [SerializeField] private SpriteRenderer  renderer;

    public void Init(int isWall)
    {
        switch (isWall)
        {
            case 0:
                renderer.color = WallColour;
                break;
            case 1:
                renderer.color = baseColour;
                break;
            case 2:
                renderer.color = ConnectColour;
                break;
            case 3:
                renderer.color = StairsColour;
                break;
            case 4:
                renderer.color = enemySpawn;
                break;
            case 5:
                renderer.color = PlayerSpawn;
                break;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Terrain";
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
