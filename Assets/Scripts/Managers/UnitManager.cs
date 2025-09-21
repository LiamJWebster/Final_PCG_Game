using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridManagerV2;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    private List<BaseEnemy> _enemies = new List<BaseEnemy>();
    public int currentEnemy;

    private List<BaseHero> _Party = new List<BaseHero>();
    private List<BaseHero> _DefeatedParty = new List<BaseHero>();

    private List<AStarNode> _movementQueue;

    private int _IDCount = 0;


    public BaseHero selectedHero;
    public BaseHero HotbarHero;

    public int[] _HeroIDs;

    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        _Party = new List<BaseHero>();
        
    }

    public void SpawnHeroes()
    {
        var heroCount = 4;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetUnitOnID<BaseHero>(i + 1);
            randomPrefab._UnitID = _IDCount;
            var spawnedHero = Instantiate(randomPrefab);

            var randomSpawnTile = GridManagerV2.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
            _Party.Add(spawnedHero);

            _IDCount++;
        }

        HotbarHero = _Party[0];
    }

    public void SpawnEnemies()
    {
        var enemycount = 5;

        for (int i = 0; i < enemycount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            randomPrefab._UnitID = _IDCount;
            var spawnedEnemy = Instantiate(randomPrefab);

            var randomSpawnTile = GridManagerV2.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);

            
            _enemies.Add(spawnedEnemy);
            SetEnemyTarget(i);

            _IDCount++;
        }
    }

    public void Combat(BaseUnit attacker, BaseUnit defender )
    {
        if (attacker.Faction == Faction.Hero)
        {
            if(attacker._ActionPoints <= 0)
            {
                GameManager.Instance.EndCombat();
                return;
            }
            attacker._ActionPoints--;
            BaseHero attackerHero = (BaseHero)attacker;
            attackerHero.ExperiencePoints++;
        }

        if (GameManager.Instance.getCombatType() == 0)//melee
        {
            defender._Hitpoints -= attacker._MeleeDamage;
        }
        else //ranged
        {
            defender._Hitpoints -= attacker._RangeDamage;
        }

        defender.TakeDamage();
        

        if (defender._Hitpoints <= 0)
        {
            if(attacker.Faction == Faction.Hero)
            {
                if(defender.Faction != Faction.Hero)
                {
                    for (int i = 0; i < _enemies.Count; i++)
                    {
                        if (_enemies[i]._UnitID == defender._UnitID)
                        {
                            _enemies.RemoveAt(i);
                        }
                    }
                }

                BaseHero attackerHero = (BaseHero)attacker;
                attackerHero.ExperiencePoints += 5;
            }
            else if (defender.Faction == Faction.Hero)//remove hero from current party and change enemy targets 
            {
                int heroPos = 0;
                for (int i = 0; i < _Party.Count; i++)
                {
                    if (defender == _Party[i])
                    {
                        heroPos = i;
                        break;
                    }
                }

                BaseHero heroCopy = (BaseHero)defender; 
                _DefeatedParty.Add(heroCopy);

                for (int i = 0; i < _enemies.Count; i++)
                {
                    int count = 0;
                    while (_enemies[i].Target == heroPos)
                    {
                        count++;
                        if(count > 50)
                        {
                            break;
                        }
                        SetEnemyTarget(i);
                    }
                }
            }
            Destroy(defender.gameObject);
        }        

        GameManager.Instance.EndCombat();
    }

    public bool enemiesClear()
    {
        if(_enemies.Count <= 0)
        {
            return true;
        }
        return false;
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
    }

    private T GetUnitOnID<T>(int ID) where T : BaseUnit
    {
        return (T)_units.Where(u => u.UnitID == ID).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        selectedHero = hero;
        HotbarHero = hero;
    }

    public void EnemyTurn ()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i]._MovementPoints = _enemies[i]._MaxMovementPoints;
            _enemies[i]._ActionPoints = _enemies[i]._MaxActionPoints;
        }

        for (int i = 0; i < _enemies.Count; i++)
        {
            currentEnemy = i;
            EnemyMovement(i);

            Vector2 pos = _enemies[i].OccupiedTile._Pos;
            Vector2 targetPos = new Vector2();

            targetPos = _Party[_enemies[i].Target].OccupiedTile._Pos;

            switch (_enemies[i].Attacktype)
            {
                case BaseEnemy.AttackPreference.Ranged:

                    break;
                case BaseEnemy.AttackPreference.Hybrid:
                    break;
                case BaseEnemy.AttackPreference.summons:
                    SummonMinions(_enemies[i]);
                    break;
                case BaseEnemy.AttackPreference.Melee:
                    break;
            }
            
            while(_enemies[i]._ActionPoints > 0)
            {
                if (Vector2.Distance(pos, targetPos) <= 1)
                {
                    Combat(_enemies[i], _Party[_enemies[i].Target]);
                }
                _enemies[i]._ActionPoints--;
            }
            
        }

    }

    public BaseEnemy GetCurrentEnemy()
    {
        return _enemies[currentEnemy];
    }

    public void SummonMinions(BaseEnemy Summoner, int NumSummons = 2)
    {
        for (int i = 0; i < NumSummons; i++) 
        {
            bool spawnTileFound = false;
            try
            {
                while (!spawnTileFound)
                {
                    var SummonPrefab = Resources.Load<ScriptableUnit>("Units/Enemies/SkeletonAxe").UnitPrefab;
                    SummonPrefab._UnitID = _IDCount;
                    var spawnedEnemy = Instantiate(SummonPrefab);

                    var randomSpawnTile = GridManagerV2.Instance.GetEnemySpawnTile(Summoner.OccupiedTile._Pos);

                    randomSpawnTile.SetUnit(spawnedEnemy);


                    _enemies.Add((BaseEnemy)spawnedEnemy);
                    SetEnemyTarget(i);

                    _IDCount++;

                    spawnTileFound = true;
                    Summoner._ActionPoints--;
                }
            }
            catch (Exception e)
            {
                Debug.Log("error summoning");
            }
            
        }
    }

    public void Movement()
    {
        if (_movementQueue != null)
        {
            if (_movementQueue.Count > 0 && _movementQueue[_movementQueue.Count - 1].position == new Vector2(0, 0))
            {
                _movementQueue.RemoveAt(_movementQueue.Count - 1);

            }
        }

    }

    public void SetMovement(List<AStarNode> Path)
    {
        _movementQueue = Path;
    }

    public void StartTurn()
    {
        for (int i = 0; i < _Party.Count; i++)
        {
            _Party[i]._MovementPoints = _Party[i]._MaxMovementPoints;
            _Party[i]._ActionPoints = _Party[i]._MaxActionPoints;
        }
    }

    public void SetEnemyTarget(int pos)
    {
        int targetPos;
        targetPos = UnityEngine.Random.Range(0, _Party.Count);

        _enemies[pos].Target = targetPos;
    }

    public void EnemyMovement(int pos)
    {
        BaseEnemy baseEnemy = _enemies[pos];

        int[] dummyArray = new int[0]; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Vector2 currentPos = baseEnemy.OccupiedTile._Pos;
        Faction UnitFaction = baseEnemy.Faction;

        Vector2 TargetPos = _Party[baseEnemy.Target].OccupiedTile._Pos;

        GridManagerV2.Instance.AStar(currentPos, baseEnemy._MovementPoints, dummyArray, TargetPos, UnitFaction);
        GridManagerV2.Instance.AstarMove(baseEnemy, baseEnemy._MovementPoints);

    }

}
