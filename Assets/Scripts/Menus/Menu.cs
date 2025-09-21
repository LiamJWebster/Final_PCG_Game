using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

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
    [SerializeField] private Sprite ElvishRanger;
    [SerializeField] private Sprite HumanHero;
    [SerializeField] private Sprite TrollHero;
    [SerializeField] private Sprite Cleric;
    [SerializeField] private Sprite DwarfFighter;
    [SerializeField] private Sprite ElvishBattleMage;

    [SerializeField] Button Continue;

    public Sprite[] AllHeroes;
    public Image[] party;
    public int partySelection = 0;

    public int[] FilledSlots;

    [SerializeField] private int numHeroes;
    private int[] HeroID;
    private int[] SelectedHeroIDs = new int [4];



    void Start()
    {
        
        party = new Image[4] {HeroOne, HeroTwo, HeroThree, HeroFour };
        AllHeroes = new Sprite[6] { ElvishRanger, HumanHero, DwarfFighter, Cleric, TrollHero, ElvishBattleMage};
        FilledSlots = new int[4] { -1, -1, -1, -1 };

        Continue.enabled = false;
        Continue.gameObject.SetActive(false);

        HeroID = new int[numHeroes];
        for(int i=0; i < numHeroes; i++)
        {
            HeroID[i] = i+1;
        }

    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Pre_Game");
    }

    public void BackToMainMenu()
    {
        Destroy(DataManager.Instance);
        SceneManager.LoadSceneAsync("Main_menu");
        
    }

    public void StartGame()
    {
        DataManager.Instance.setParty(SelectedHeroIDs);
        SceneManager.LoadSceneAsync("Testing_Ground"); //to do need to set to load into scene        
    }

    public void SetSelectedHeroImage(Sprite newSprite, Image SelectedSlot)
    {
        SelectedSlot.sprite = newSprite;
        SelectedSlot.color = new Color32(255,255, 255, 255);
    }

    public void SetSelectedHero(int SelectedHero)
    {
        bool alreadySelected = false;
        //bool fullParty = true;

        int emptySlots = 0;

        for (int i = 0; i < FilledSlots.Length; i++)
        {
            if (FilledSlots[i] == SelectedHero)
            {
                alreadySelected = true;
            }
            else if (FilledSlots[i] == -1)
            {
                emptySlots += 1;
            }
        }

        if (!alreadySelected)
        {
            FilledSlots[partySelection] = SelectedHero;
            SetSelectedHeroImage(AllHeroes[SelectedHero-1], party[partySelection]);
            SelectedHeroIDs[partySelection] = SelectedHero;

            if (emptySlots < 2)
            {
                Continue.enabled = true;
                Continue.gameObject.SetActive(true);
            }
        }  
    }

    public void SetPartySelect(int SelectedSlot)
    {
        partySelection = SelectedSlot;
    }
}
