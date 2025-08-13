using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static GridManager; why is this here? can't remember adding it

public class Menu : MonoBehaviour
{
    //[SerializeField] private Text Detail_Heading;
    //[SerializeField] private Enemy enemyPrefab;

    //[SerializeField] Sprite newSprite;
    //[SerializeField] SpriteRenderer spriteRenderer;
    //[SerializeField] private SpriteRenderer spriteRenderer;

    [Header ("Player Party UI")]
    [SerializeField] Image HeroOne;
    [SerializeField] Image HeroTwo;
    [SerializeField] Image HeroThree;
    [SerializeField] Image HeroFour;

    [Header("Hero Images")]
    [SerializeField] Sprite ElvishRanger;
    [SerializeField] Sprite HumanHero;
    [SerializeField] Sprite TrollHero;
    [SerializeField] Sprite Cleric;
    [SerializeField] Sprite DwarfFighter;
    [SerializeField] Sprite ElvishBattleMage;

    [SerializeField] Button Continue;

    public Sprite[] AllHeroes;
    public Image[] party;
    public int partySelection = 0;

    public int[] FilledSlots;



    void Start()
    {
        
        party = new Image[4] {HeroOne, HeroTwo, HeroThree, HeroFour };
        AllHeroes = new Sprite[6] { ElvishRanger, HumanHero, TrollHero, Cleric, DwarfFighter, ElvishBattleMage};
        FilledSlots = new int[4] { -1, -1, -1, -1 };

        Continue.enabled = false;
        Continue.gameObject.SetActive(false);

    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Pre_Game");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync("Main_menu");
    }

    public void SetSelectedHero(Sprite newSprite, Image SelectedSlot)
    {
        SelectedSlot.sprite = newSprite;
        SelectedSlot.color = new Color32(255,255, 255, 255);
    }

    public void SetSelectedHero(int SelectedHero)
    {
        bool alreadySelected = false;
        //bool fullParty = true;

        int emptySlots = 0;
        /*
        for (int i = 0; i < FilledSlots.Length; i++)
        {
            if (FilledSlots[i] == SelectedHero)
            {
                alreadySelected = true;
            }
            if(FilledSlots[i] == -1)
            {
                fullParty = false;
                emptySlots +=1;
            }
        }
        */
        for (int i = 0; i < FilledSlots.Length; i++)
        {
            if (FilledSlots[i] == SelectedHero)
            {
                alreadySelected = true;
            }
            if (FilledSlots[i] == -1)
            {
                emptySlots += 1;
            }
        }

        if (!alreadySelected)
        {
            FilledSlots[partySelection] = SelectedHero;
            SetSelectedHero(AllHeroes[SelectedHero], party[partySelection]);

            

            if (emptySlots < 2)
            {
                Continue.enabled = true;
                Continue.gameObject.SetActive(true);
                //enable continue
                Debug.Log("yoyoyo party is full ");
            }
        }        
        //Debug.Log(AllHeroes.Length);
    }

    public void SetPartySelect(int SelectedSlot)
    {
        partySelection = SelectedSlot;
    }
}
