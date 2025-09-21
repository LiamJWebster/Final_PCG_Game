using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dungeon_generation;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private List<List<DungeonNode>> _nodes;
    private int selectedRow = 0;
    private int selectedCol = 0;
    public bool HasDungeon = false;

    [SerializeField] public int[] HeroIDs = new int[4];
    [SerializeField] public int[] HeroExperience = new int[4];

    public HeroStats[] PartySave = new HeroStats[4];

    public bool loadDungeon;

    public bool Started =false;

    public class HeroStats
    {
        public int HeroID;

        public int HeroExperience;
        public int Melee;
        public int Ranged;

        public int maxMovement;
        public int MaxHealth;
        public int MaxExperience;

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void setParty(int[] party)
    {
        HeroIDs = party;
    }

    public void SaveParty()
    {
        for (int i = 0; i < PartySave.Length; i++)
        {

        }
    }

    public void SaveDungeon(List<List<DungeonNode>> nodes, int col, int row)
    {
        _nodes = nodes;
        selectedCol = col;
        selectedRow = row;
        HasDungeon = true;
    }

    public void GenerateDungeon()
    {
        List<List<DungeonNode>> updatedDungeon = new List<List<DungeonNode>>();

        for (int i = 0; i < _nodes.Count; i++) 
        {
            List<DungeonNode> Column = new List<DungeonNode>();
            for (int j = 0; j < _nodes[i].Count; j++) 
            {
                if(i > selectedCol)
                {
                    Column.Add(_nodes[i][j]);
                }
                else if (i == selectedCol && j == selectedRow)
                {
                    Column.Add(_nodes[i][j]);
                }
            }
            if (Column.Count > 0)
            {
                updatedDungeon.Add(Column);
            }
        }        

        Dungeon_generation.Instance.Renderlayout(updatedDungeon);

    }

}
