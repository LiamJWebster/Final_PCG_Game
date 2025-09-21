using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public int Target;

    public AttackPreference Attacktype;

    public enum AttackPreference
    {
        Melee = 0,
        Ranged  = 1,
        magic = 2,
        summons = 3,
        Hybrid = 4,
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UnitAI()
    {

    }

    
}
