using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    private ItemStats _Itemstats;

    [Header("Item Stats")]
    [SerializeField] private int Movement;
    [SerializeField] private int Health;
    [SerializeField] private int Armour;
    [SerializeField] private int Melee;
    [SerializeField] private int Ranged;
    [SerializeField] private int ActionPoints;

    public class ItemStats
    {
        public int Movement;
        public int Health;
        public int Armour;
        public int Melee;
        public int Ranged;
        public int ActionPoints;
    }

    void Awake()
    {
        _Itemstats = new ItemStats();

        _Itemstats.Movement = Movement;
        _Itemstats.Health = Health;
        _Itemstats.Armour = Armour;
        _Itemstats.Melee = Melee;
        _Itemstats.Ranged = Ranged;
        _Itemstats.ActionPoints = ActionPoints;         
    }

    public ItemStats ItemPickup()
    {
        return _Itemstats;
    }
}
