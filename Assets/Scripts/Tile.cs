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

    [SerializeField] private SpriteRenderer  Renderer;

    public void Init(int isWall)
    {
        switch (isWall)
        {
            case 0:
                Renderer.color = WallColour;
                break;
            case 1:
                Renderer.color = baseColour;
                break;
            case 2:
                Renderer.color = ConnectColour;
                break;
            case 3:
                Renderer.color = StairsColour;
                break;
            case 4:
                Renderer.color = enemySpawn;
                break;
            case 5:
                Renderer.color = PlayerSpawn;
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
