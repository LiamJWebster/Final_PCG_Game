using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;

    public Faction Faction;

    public int _Hitpoints;
    public int _MaxHitpoints;

    public int _ActionPoints;
    public int _MaxActionPoints;

    public int _MovementPoints;
    public int _MaxMovementPoints;

    public int _MeleeDamage;
    public int _RangeDamage;

    public int _UnitID;

    [SerializeField] private Slider HealthBar;


    public void Healing(int Hitpoints)
    {
        _Hitpoints += Hitpoints;
        _Hitpoints = Mathf.Min(Hitpoints, _MaxHitpoints);

    }

    public class EffectOverTime
    {
        public int TurnsRemaining;
        
        public int HPEffect;
        public int ActionEffect;
        public int MovementEffect;
    }

    public void TakeDamage(int dmg = 0)
    {
        _Hitpoints -= dmg;
        HealthBar.maxValue = _MaxHitpoints;
        HealthBar.value = _Hitpoints;
        //HealthBar.maxValue = _MaxHitpoints;
    }
}
