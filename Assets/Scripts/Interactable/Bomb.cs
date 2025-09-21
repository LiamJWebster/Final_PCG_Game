using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bomb : BaseObject
{
    //[SerializeField]  private AudioClip explosion; 
    [SerializeField] private AudioSource AudioSource;
    
    // Start is called before the first frame update
    void Awake()
    {
        Walkable = false;
    }

    public void explode(int explosionRadius = 1)
    {
        //explosion.
        AudioManager.Instance.PlayAudio();

        int startX = (int)_tile._Pos.x - explosionRadius;
        int startY = (int)_tile._Pos.y - explosionRadius;

        int endX = (int)_tile._Pos.x + 1 + explosionRadius;
        int endY = (int)_tile._Pos.y + 1 + explosionRadius;

        BaseObject fire = Resources.Load<EnviromentalObject>("Objects/Fire").ObjectPrefab;
        List<BaseObject> list = new List<BaseObject>();

        List<Fire> Flames = new List<Fire>();

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                
                int xPos = Mathf.Min(GridManagerV2.Instance._width - 1, Mathf.Max(x, 0));
                int yPos = Mathf.Min(GridManagerV2.Instance._height - 1, Mathf.Max(y, 0));
                Vector2 pos = new Vector2(xPos, yPos);
                Tile tile = GridManagerV2.Instance.GetTileAtPosition(pos);
                //do dmage to units in radius 
                //set fire to other bombs

                if (tile.IsWalkable())//&& tile.enviromentalEffect.GetType != Fire
                {
                    if (tile.OccupiedUnit != null)
                    {
                        tile.OccupiedUnit.TakeDamage(5);
                    }

                    if (tile.enviromentalEffect == null)
                    {
                        var spawnedItem = Instantiate(fire);
                        tile.SetObject(spawnedItem);
                        Flames.Add((Fire)spawnedItem);
                    }
                    else
                    {
                        if (tile.enviromentalEffect.Flamable)
                        {
                            list.Add(tile.enviromentalEffect);                          
                        }
                    }
                        
                }                           
                               
            }
        }

        var selfFire = Instantiate(fire);
        this._tile.SetObject(selfFire);
        Flames.Add((Fire)selfFire);

        GameManager.Instance.AddFlames(Flames);

        Destroy(gameObject);
        while (list.Count > 0)
        {
            Bomb grenade = (Bomb)list[0];
            grenade.explode(2);
            list.RemoveAt(0);
        }
        
    }
}
