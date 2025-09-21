using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Image HeroPortrait;
    [SerializeField] Slider HeroHealthBar;
    [SerializeField] TMP_Text HeroHealthText;

    [SerializeField] Slider HeroXPBar;
    [SerializeField] TMP_Text HeroXPText;

    [SerializeField] Slider HeroAPBar;
    [SerializeField] TMP_Text HeroAPText;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

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
}
