using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{  

    public static GameManager Instance;
    [SerializeField] private GridManagerV2 GridManagerV2 = null;
    [SerializeField] public GameObject Highlight = null;

    public Bar_Slider healthbar;

    public GameState State;    

    private List<Vector2> CombatTiles = new List<Vector2>();
    private bool CombatActive = false;
    private int combatType = 0;

    private List<Fire> Flames = new List<Fire>(); // can be expanded later just fire for now but if/when adding other srufaces 
    private int _FireID = 0;

    /*
    [SerializeField] Image HeroPortrait;
    [SerializeField] Slider HeroHealthBar;
    [SerializeField] TMP_Text HeroHealthText;

    [SerializeField] Slider HeroXPBar;
    [SerializeField] TMP_Text HeroXPText;

    [SerializeField] Slider HeroAPBar;
    [SerializeField] TMP_Text HeroAPText;
    */

    private bool exitSet = false;
    public bool inGame = false;
    private bool started = false;

    //[SerializeField] public AudioListener AudioListener;
    //[SerializeField] public AudioSource AudioSource;

    //private bool TestFire = false;
    private int[,] TestGrid;


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

        //DontDestroyOnLoad(Instance);

    }

    public void ChangeState(GameState newState)
    {
        State = newState;
        switch (newState) 
        {
            case GameState.NewArea:
                inGame = false;
                Instance.ChangeState(GameState.GenerateRoom);
                break;
            case GameState.GenerateRoom:                
                GridManagerV2.GenerateTestingRoom(GridManagerV2._width, GridManagerV2._height, GridManagerV2._minSize, GridManagerV2._MaxX, GridManagerV2._MaxY);                                    
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                Instance.ChangeState(GameState.SpawnEnemies);
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                Instance.ChangeState(GameState.SpawnItems);
                break;
            case GameState.PlayerTurn:
                UnitManager.Instance.StartTurn();
                inGame = true;
                break;
            case GameState.EvilTurn:
                UnitManager.Instance.EnemyTurn();
                Instance.ChangeState(GameState.Enviroment);
                break;
            case GameState.NeutralTurn:
                break;
            case GameState.SpawnItems:
                ItemManager.Instance.SpawnItems(4);
                ItemManager.Instance.SpawnObjects(5);
                Instance.ChangeState(GameState.PlayerTurn);
                break;
            case GameState.Enviroment:
                FlamesTurn();
                endGame();
                break;
            case GameState.LoadDungeon:
                DataManager.Instance.GenerateDungeon();
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
        SpawnItems = 6,
        Enviroment =7,
        LoadDungeon = 8,
        NewArea = 9,
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "Testing_Ground")
        {
            ChangeState(GameState.GenerateRoom);
            started = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && inGame)
        {
            if (Input.GetKeyDown("space"))
            {
                // healthbar.SetCurrentValue(5);
                TestGrid = GridManagerV2.Instance.tester;
                GridManagerV2.TestAutomata(TestGrid);
            }

            if (Input.GetMouseButtonDown(1))
            {                
                UnitManager.Instance.selectedHero = null;
                GridManagerV2.Instance.CleanMovementHighlight();

                CombatComplete();                  
            }

            if (Input.GetMouseButtonDown(0))
            {
                UIManager.Instance.UpdateUI(UnitManager.Instance.HotbarHero);
            }

        }        

        //UnitManager.Instance.Movement();

    }

    public void EndTurn()
    {
        if (State == GameState.PlayerTurn)
        {
            Instance.ChangeState(GameState.EvilTurn);
        }        
    }

    public void endGame() 
    {
        if (UnitManager.Instance.enemiesClear() && !exitSet)
        {
            ItemManager.Instance.SpawnExit();
            exitSet = true;
        }
        else if (exitSet && ItemManager.Instance.CheckExit())
        {
            inGame = false;
            //SceneManager.LoadSceneAsync("Dungeon_layout");
            SceneManager.LoadSceneAsync("Testing_Ground");
            //return;
            
        }
        Instance.ChangeState(GameState.PlayerTurn);

    }

    /*
    public void UpdateUI(BaseHero newHero)
    {
        HeroPortrait.sprite = newHero.Sprite;
        
        HeroHealthBar.maxValue = newHero._MaxHitpoints;
        HeroHealthBar.value = newHero._Hitpoints;

        HeroXPBar.maxValue = newHero._MaxMovementPoints;
        HeroXPBar.value = newHero._MovementPoints;

        //HeroXPBar.maxValue = newHero.MaxExperiencePoints;
        //HeroXPBar.value = newHero.ExperiencePoints;

        HeroAPBar.maxValue = newHero._MaxActionPoints;
        HeroAPBar.value = newHero._ActionPoints;

        String newText;
        newText = newHero._Hitpoints + "/" + newHero._MaxHitpoints;
        HeroHealthText.text = newText;  
    }
    */

    // these two combat functions can probably be cleaned up and possibly merged a bit with input from the buttons, doubt I'll have time though 
    public void MeleeAttack()
    {
        if (UnitManager.Instance.HotbarHero != null && State == GameState.PlayerTurn)
        {
            List<Vector2> tilesToHighlight = new List<Vector2>();
            Vector2 Heropos = UnitManager.Instance.HotbarHero.OccupiedTile._Pos;
            for (int x = (int)Heropos.x - 1; x < (int)Heropos.x + 2; x++) 
            {
                for (int y = (int)Heropos.y - 1; y < (int)Heropos.y + 2; y++)
                {
                    if(((int)Heropos.x != x || (int)Heropos.y != y) && GridManagerV2.Instance.GetTileAtPosition(new Vector2(x,y)).IsWalkable()) //exclude unwalkable tiles and the base tile of the hero
                    {
                        tilesToHighlight.Add(new Vector2(x, y));
                    }
                }
            }
            GridManagerV2.Instance.HighlightAttack(tilesToHighlight);

            CombatTiles = tilesToHighlight;
            CombatActive = true;
            combatType = 0;
        }        

    }

    public void RangedAttack()
    {
        if (UnitManager.Instance.HotbarHero != null && State == GameState.PlayerTurn)
        {
            List<Vector2> tilesToHighlight = new List<Vector2>();
            Vector2 Heropos = UnitManager.Instance.HotbarHero.OccupiedTile._Pos;
            for (int x = (int)Heropos.x - 4; x < (int)Heropos.x + 5; x++)
            {
                for (int y = (int)Heropos.y - 4; y < (int)Heropos.y + 5; y++)
                {
                    int Adjustedx = Mathf.Min(GridManagerV2.Instance._width-1, Mathf.Max(x, 0));
                    int Adjustedy = Mathf.Min(GridManagerV2.Instance._height-1, Mathf.Max(y, 0));
                    if (((int)Heropos.x != Adjustedx || (int)Heropos.y != Adjustedy) && GridManagerV2.Instance.GetTileAtPosition(new Vector2(Adjustedx, Adjustedy)).IsWalkable()) //exclude unwalkable tiles and the base tile of the hero
                    {
                        tilesToHighlight.Add(new Vector2(Adjustedx, Adjustedy));
                    }
                }
            }
            GridManagerV2.Instance.HighlightAttack(tilesToHighlight);

            CombatTiles = tilesToHighlight;
            CombatActive = true;
            combatType = 1;
        }

    }

    public void EndCombat()
    {
        CombatActive = false;
        GridManagerV2.Instance.CleanMovementHighlight();
    }

    public int getCombatType()
    {
        return combatType;
    }

    private void FlamesTurn()
    {
        for (int i = Flames.Count-1; i >= 0; i--)
        {
            Flames[i].Time();
        }
    }

    public List<Fire> GetFlames()
    {
        return Flames;
    }

    public void clearFire(int pos)
    {
        Flames.RemoveAt(pos); 
    }

    public void AddFlames(List<Fire> toAdd)
    {
        for (int i = 0; i < toAdd.Count; i++) 
        {
            Fire fire = toAdd[i];
            fire.FireID = _FireID;
            Flames.Add(fire);
            _FireID++;
        }
    }

    public void UseHealthPotion()
    {
        Debug.Log("GLUG");
    }

    public bool GetCombatActive()
    {
        return CombatActive;
    }

    public List<Vector2> GetCombatTiles()
    {
        return CombatTiles;
    }

    public void CombatComplete()
    {
        CombatActive = false;
        CombatTiles.Clear();
    }

}
