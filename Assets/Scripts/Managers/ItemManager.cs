using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class ItemManager : MonoBehaviour
{
    
    public static ItemManager Instance;

    private List<ScriptableItem> _items;
    private List<EnviromentalObject> _Objects;

    private BaseObject _exit;
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

        _items = Resources.LoadAll<ScriptableItem>("Items/Weapons").ToList();
        //_Objects = Resources.LoadAll<EnviromentalObject>("Objects").ToList();
        _Objects = new List<EnviromentalObject>();
        _Objects.Add(Resources.Load<EnviromentalObject>("Objects/Bomb"));
        _Objects.Add(Resources.Load<EnviromentalObject>("Objects/Bomb"));
    }

    public void SpawnItems(int numItems)
    {
        for (int i = 0; i < numItems; i++)
        {
            var randomPrefab = GetRandomItem();
            var spawnedItem = Instantiate(randomPrefab);

            var randomSpawnTile = GridManagerV2.Instance.GetItemSpawn();

            randomSpawnTile.SetItem(spawnedItem);
        }
    }

    public void SpawnObjects(int numObjects)
    {
        for (int i = 0; i < numObjects; i++)
        {
            var randomPrefab = GetRandomObject();
            var spawnedObject = Instantiate(randomPrefab);

            var randomSpawnTile = GridManagerV2.Instance.GetItemSpawn();

            randomSpawnTile.SetObject(spawnedObject);
        }
    }

    public void SpawnExit()
    {
        var randomPrefab = Resources.Load<EnviromentalObject>("Objects/Trapdoor").ObjectPrefab;
        var spawnedItem = Instantiate(randomPrefab);

        var randomSpawnTile = GridManagerV2.Instance.GetItemSpawn();

        randomSpawnTile.SetObject(spawnedItem);
        _exit = spawnedItem;
    }

    public bool CheckExit()
    {
        if(_exit._tile.OccupiedUnit != null)
        {
            return true;            
        }
        return false;
    }

    private BaseItem GetRandomItem()
    {
        BaseItem item = _items[Random.Range(0, _items.Count)].ItemPrefab;

        return item;
    }

    private BaseObject GetRandomObject()
    {
        BaseObject item = _Objects[Random.Range(0, _Objects.Count - 1)].ObjectPrefab;

        return item;
    }

    /*
    private T GetItemOnID<T>(int ID) where T : BaseUnit
    {
        return (T)_items.Where(u => u.UnitID == ID).First().UnitPrefab;
    }*/


}

