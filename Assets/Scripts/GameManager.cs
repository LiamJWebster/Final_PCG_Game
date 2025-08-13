using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Bar_Slider healthbar;

    public static GameManager Instance;

    public GameState State;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    public void ChangeState(GameState newState)
    {
        State = newState;
        switch (newState) 
        {
            case GameState.GenerateRoom:
                break;
            case GameState.SpawnHeroes:
                break;
            case GameState.SpawnEnemies:
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EvilTurn:
                break;
            case GameState.NeutralTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState));
        }

    }

    public enum GameState 
    {
        GenerateRoom = 0,
        SpawnHeroes = 1,
        SpawnEnemies = 2,
        PlayerTurn = 3,
        EvilTurn = 4,
        NeutralTurn = 5,

    
    }

    void Start()
    {
        healthbar.SetMaxValue(50);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            healthbar.SetCurrentValue(5);
        }

    }

    
}
