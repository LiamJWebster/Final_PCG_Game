using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public int Hitpoints = 1;
    public Tile _tile;

    public bool Walkable = false;
    public bool Flamable = false;

    public int ObjectID;

    public void Instantiate(Tile tile)
    {
        _tile = tile;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage()
    {
        Debug.Log("damage");
    }
}
