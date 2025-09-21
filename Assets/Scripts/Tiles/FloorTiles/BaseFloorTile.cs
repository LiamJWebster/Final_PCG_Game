using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFloorTile : MonoBehaviour
{
    [SerializeField] private Color _baseColour;
    [SerializeField] protected SpriteRenderer _Renderer;
    [SerializeField] protected SpriteRenderer _HighlightRenderer;
    [SerializeField] private GameObject _HighLight;


    // Start is called before the first frame update
    void Start()
    {
        
    }
}
