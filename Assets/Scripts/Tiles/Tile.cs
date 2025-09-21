using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseItem;
using static UnityEngine.UI.CanvasScaler;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColour;
    [SerializeField] protected SpriteRenderer  _Renderer;
    [SerializeField] protected SpriteRenderer _HighlightRenderer;
    
    [SerializeField] protected GameObject _Movement;
    [SerializeField] private GameObject _HighLight;

    [SerializeField] private bool _isWalkable;

    public BaseObject enviromentalEffect;

    public BaseUnit OccupiedUnit;
    public Vector2 _Pos;

    public BaseItem OccupiedItem;

    public enum tileType
    {
        floor,
        water,
        wall
    }

    public tileType _tileType;

    public virtual void Init(int isWall, Vector2 pos)
    {
        switch (isWall)
        {
            case 0:
                _Renderer.color = _baseColour;
                break;
            case 1:
                _Renderer.color = _baseColour;
                break;
            case 2:
                _Renderer.color = _baseColour;
                break;
            case 3:
                _Renderer.color = _baseColour;
                break;
            case 4:
                _Renderer.color = _baseColour;
                break;
            case 5:
                _Renderer.color = _baseColour;
                break;

        }

        _Pos = pos;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Terrain";
    }


    public bool IsClear => _isWalkable && OccupiedUnit == null;

    public bool IsWalkable()
    {
        return _isWalkable;
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        UnitManager.Instance.SetMovement(GridManagerV2.Instance.GetPath());
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public BaseUnit GetUnit() { return OccupiedUnit; }

    private void OnMouseEnter()
    {
        if (!GameManager.Instance.GetCombatActive())
        {
            _HighLight.SetActive(true);
            GameManager.Instance.Highlight.transform.position = transform.position;
            GameManager.Instance.Highlight.transform.position += new Vector3((float)-0.5, (float)-0.5);
            GameManager.Instance.Highlight.SetActive(true);
            if (OccupiedUnit == null)
            {
                SelectHighlight.instance.NeutralColour();
                if (UnitManager.Instance.selectedHero != null && IsWalkable())
                {
                    GridManagerV2.Instance.CleanMovementHighlight();
                    int[] dummyArray = new int[0]; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    Vector2 heroPosition = UnitManager.Instance.selectedHero.OccupiedTile._Pos;
                    int HeroMovement = UnitManager.Instance.selectedHero._MovementPoints;

                    GridManagerV2.Instance.Djikstra(heroPosition, HeroMovement, dummyArray);

                    GridManagerV2.Instance.AStar(heroPosition, HeroMovement, dummyArray, _Pos, UnitManager.Instance.selectedHero.Faction);
                    GridManagerV2.Instance.DrawAstar(HeroMovement);
                }
            }
            else if (OccupiedUnit.Faction == Faction.Hero)
            {
                SelectHighlight.instance.FriendlyColour();
                // GridManagerV2.Instance.MovementHighlight(_Pos,OccupiedUnit.MovementPoints);
                int[] dummyArray = new int[0]; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                GridManagerV2.Instance.Djikstra(_Pos, OccupiedUnit._MovementPoints, dummyArray);
            }
            else
            {
                SelectHighlight.instance.EnemyColour();
            }
        }
    }

    private void OnMouseExit()
    {
        if (!GameManager.Instance.GetCombatActive())
        {
            _HighLight.SetActive(false);
            GameManager.Instance.Highlight.SetActive(false);

            if (OccupiedUnit != null)
            {
                GridManagerV2.Instance.CleanMovementHighlight();
            }

            if (UnitManager.Instance.selectedHero != null)
            {
                GridManagerV2.Instance.EraseAstar();
            }
        }
        else 
        {

        }            

    }

    public void ActivateHighlight()
    {
        _HighLight.SetActive(true);
    }

    public void ActivateHighlight(Color color)
    {
        _HighLight.SetActive(true);
        _HighlightRenderer.color = color;   
    }

    public void DeactivateHighlight()
    {
        _HighLight.SetActive(false);
    }

    public void DeactivateHighlight(Color color)
    {
        _HighLight.SetActive(false);
        _HighlightRenderer.color = color;
    }

    public void ActivateMovement()
    {
        _Movement.SetActive(true);
    }

    public void DeactivateMovement()
    {
        _Movement.SetActive(false);
    }

    public BaseItem GetItem() 
    {
        return OccupiedItem; 
    }

    public void SetItem(BaseItem item) 
    {
        OccupiedItem = item;
        
        OccupiedItem.transform.position = transform.position;
    }

    public void SetObject(BaseObject Object)
    {
        enviromentalEffect = Object;
        Object._tile = this;

        enviromentalEffect.transform.position = transform.position;
    }

    public BaseObject GetObject()
    {
        return enviromentalEffect;
    }

    public bool ObjectWalkable()
    {
        if (enviromentalEffect == null)
        {
            return true;
        }
        else
        {
            if (enviromentalEffect.Walkable == false)
            {
                return false;
            }
            return true;
        }
    }

    public void ItemPickup(ItemStats item)
    {
        UnitManager.Instance.selectedHero._MaxActionPoints += item.ActionPoints;
        UnitManager.Instance.selectedHero._MeleeDamage += item.Melee;
        UnitManager.Instance.selectedHero._RangeDamage += item.Ranged;
        UnitManager.Instance.selectedHero._MaxHitpoints += item.Health;
        UnitManager.Instance.selectedHero._MaxMovementPoints += item.Movement;
        UnitManager.Instance.selectedHero.armour += item.Armour;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.State != GameManager.GameState.PlayerTurn)
        {
            return;
        }

        /*
        if (enviromentalEffect != null)
        {
            Bomb grenade = (Bomb)enviromentalEffect;

            grenade.explode(2);

        }
        */ //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Need to move to a function when the bomb takes damage

        if (OccupiedUnit != null )
        {
            if (OccupiedUnit.Faction == Faction.Hero)
            {
                BaseHero baseHero = (BaseHero)OccupiedUnit;
                UnitManager.Instance.SetSelectedHero(baseHero);
                UIManager.Instance.UpdateUI(baseHero);
            }
            else
            {
                if (GameManager.Instance.GetCombatActive())
                {
                    var enemy = (BaseEnemy)OccupiedUnit;
                    //combat
                    UnitManager.Instance.Combat(UnitManager.Instance.HotbarHero, enemy);
                    
                    UnitManager.Instance.selectedHero = null;
                }
            }
        }
        else if(!GameManager.Instance.GetCombatActive())
        {
            if(UnitManager.Instance.selectedHero != null && _isWalkable)
            {
                if (OccupiedItem != null)
                {
                    //ItemStats itemStats = OccupiedItem.ItemPickup();
                    UnitManager.Instance.selectedHero.ItemPickup(OccupiedItem.ItemPickup());
                    Destroy(OccupiedItem.gameObject);
                }

                GridManagerV2.Instance.AstarMove(UnitManager.Instance.selectedHero, UnitManager.Instance.selectedHero._MovementPoints);
            }
        }

        if(enviromentalEffect != null && GameManager.Instance.GetCombatActive())
        {
            Bomb grenade = (Bomb)enviromentalEffect;

            grenade.explode(2);
        }

    }

}
