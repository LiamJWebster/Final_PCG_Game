using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private GameObject _SelectedPortrait;
    [SerializeField] private GameObject _SelectedHitpoints;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateSelectedHero(BaseHero hero)
    {
        if(hero == null)
        {
            _SelectedPortrait.SetActive(false);
            return;
        }

        //_SelectedPortrait.GetComponentInChildren<Text>().text = hero.UnitName;
        _SelectedPortrait.SetActive(true);


    }
}
