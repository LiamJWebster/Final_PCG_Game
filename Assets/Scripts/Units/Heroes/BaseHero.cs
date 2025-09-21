using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseItem;

public class BaseHero : BaseUnit
{
    public int ExperiencePoints;
    public int MaxExperiencePoints;


    public int armour;

    public Sprite Sprite;

    public void ItemPickup(ItemStats item)
    {
        _MaxActionPoints += item.ActionPoints;
        _MeleeDamage += item.Melee;
        _RangeDamage += item.Ranged;
        _MaxHitpoints += item.Health;
        _MaxMovementPoints += item.Movement;
        armour += item.Armour;
    }
}
