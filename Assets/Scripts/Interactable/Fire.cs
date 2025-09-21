using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : BaseObject
{
    [SerializeField] private int burnTime = 2;
    public int FireID;
    // Start is called before the first frame update
    void Awake()
    {
        Walkable = true;
    }

    public void Time()
    {
        burnTime -= 1;
        if(_tile.OccupiedUnit != null)
        {
            _tile.OccupiedUnit.TakeDamage(2);
        }
        //check units do damage to them if they are on the tile
        if (burnTime == 0)
        {
            List<Fire> Fires = GameManager.Instance.GetFlames();
            int myPos = 0;
            for(int i=0; i < Fires.Count; i++)
            {
                if (Fires[i].FireID == FireID)
                {
                    myPos = i;
                }
            }
            GameManager.Instance.clearFire(myPos);
            Destroy(gameObject);
        }
    }


}
