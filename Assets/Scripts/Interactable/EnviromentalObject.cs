using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Enviromental Object")]

public class EnviromentalObject : ScriptableObject
{
    public ObjectType ObjectType;
    public BaseObject ObjectPrefab;
    public int ObjectID;
}

public enum ObjectType
{
    Destructable = 0,
    Trigger = 1,
}
