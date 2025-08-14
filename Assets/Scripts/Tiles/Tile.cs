using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColour;

    [SerializeField] protected SpriteRenderer  Renderer;

    public virtual void Init(int isWall)
    {
        switch (isWall)
        {
            case 0:
                Renderer.color = baseColour;
                break;
            case 1:
                Renderer.color = baseColour;
                break;
            case 2:
                Renderer.color = baseColour;
                break;
            case 3:
                Renderer.color = baseColour;
                break;
            case 4:
                Renderer.color = baseColour;
                break;
            case 5:
                Renderer.color = baseColour;
                break;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Terrain";
    }

}
