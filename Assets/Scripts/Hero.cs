using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Hero Stats")]
    [SerializeField] public int HealthPoints;
    [SerializeField] public int MovementPoints;
    [SerializeField] public int ActionPoints;
    [SerializeField] public int ExperiencePoints;

    [Header("Hero Equipent")]
    [SerializeField] public int Head;
    [SerializeField] public int Chest;
    [SerializeField] public int Hands;
    [SerializeField] public int Legs;
    [SerializeField] public int Feet;
    /*
    [SerializeField] public int Amulet;
    [SerializeField] public int RingOne;
    [SerializeField] public int RingTwo;
    [SerializeField] public int Belt;
    */

    [Header("Hero Weapons")]
    [SerializeField] public int LeftHand;
    [SerializeField] public int RightHand;

    [SerializeField] public int LeftReserve;
    [SerializeField] public int RightReserve;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
