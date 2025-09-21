using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Item")]

public class ScriptableItem : ScriptableObject
{
    public ItemType ItemType;
    public BaseItem ItemPrefab;
    public int ItemID;
}

public enum ItemType
{
    Inventory = 0,
    WorldObject = 1,
    Trigger = 2
}