using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectHighlight : MonoBehaviour
{
    public static SelectHighlight instance;

    [SerializeField] protected SpriteRenderer _Top;
    [SerializeField] protected SpriteRenderer _Right;
    [SerializeField] protected SpriteRenderer _Left;
    [SerializeField] protected SpriteRenderer _Bottom;

    [SerializeField] private Color _neutralColour;
    [SerializeField] private Color _enemyColour;
    [SerializeField] private Color _friendlyColour;

    private void Awake()
    {
        instance = this;
    }

    public void EnemyColour()
    {
        _Top.color = _enemyColour;
        _Right.color = _enemyColour;
        _Left.color = _enemyColour;
        _Bottom.color = _enemyColour;
    }

    public void NeutralColour()
    {
        _Top.color = _neutralColour;
        _Right.color = _neutralColour;
        _Left.color = _neutralColour;
        _Bottom.color = _neutralColour;
    }

    public void FriendlyColour()
    {
        _Top.color = _friendlyColour;
        _Right.color = _friendlyColour;
        _Left.color = _friendlyColour;
        _Bottom.color = _friendlyColour;
    }
}
