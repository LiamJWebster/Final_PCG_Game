using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dungeon_generation : MonoBehaviour
{
    public static Dungeon_generation Instance;

    [SerializeField] private int NumberFloors;
    [SerializeField] private int minFloors;
    [SerializeField] private int maxFloors;
    [SerializeField] private int height;

    [SerializeField] private circle_floor circlePrefab;

    private List<List<DungeonNode>> _nodes = new List<List<DungeonNode>>();
    private int currentCol = 0;
    private int currentRow = 0;

    [SerializeField] private Color _lineColor;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Material _material;

    public class DungeonNode
    {
        public float x;
        public float y;

        public int row;
        public int col;

        public enum Difficulty
        {
            friendly = 0,
            neutral = 1,
            abandoned = 2,
            Hostile = 3
        }

        public Difficulty NodeType;

        public List<Vector2> connections;

        public bool selectable;
    }

    void GenerateDungeon(int numFloors = 8)
    {
        //int x = 0;
        int[] floorSizes = new int[numFloors];
        int[] floorConnectionSize = new int[3];

        
        int previousNode = 3;
        floorSizes[0] = 1;
        floorSizes[1] = 3;
        floorSizes[floorSizes.Length - 2] = 3;
        floorSizes[floorSizes.Length - 1] = 1;
        for (int i = 2; i < floorSizes.Length - 2; i++)
        {
            if (previousNode == 3)
            {
                floorSizes[i] = Random.Range(previousNode, previousNode + 2);

            }
            else if (previousNode == 4)
            {
                floorSizes[i] = Random.Range(previousNode - 1, previousNode + 2);

            }
            else if (previousNode == 5)
            {
                floorSizes[i] = Random.Range(previousNode - 1, previousNode + 1);

            }
            previousNode = floorSizes[i];
        }        
        
        _nodes = generateNode(floorSizes,  2);       

    }

    public List<List<DungeonNode>> generateNode(int[]dungeon, int x)
    {
        List<List<DungeonNode>> Nodes = new List<List<DungeonNode>>();
        for (int i = 0; i < dungeon.Length; i++)
        {
            

            int xPos = i * 2;
            int yPos = (int)Mathf.Floor(dungeon[i] / 2);
            float yStart = -3 * yPos;
            yStart += 4;
            if (dungeon[i] % 2 == 0)
            {
                yStart = yStart + 1.5f;
            }


            List<DungeonNode> Column = new List<DungeonNode>();

            for (int j = 0; j < dungeon[i]; j++)
            {              

                DungeonNode Node = new DungeonNode();
                Node.connections = new List<Vector2>();

                Node.x = xPos;
                Node.y = yStart;

                Node.row = j;
                Node.col = i;

                yStart += 3;

                if(i == 0)
                {
                    Node.selectable = false;
                }
                else
                {
                    Node.selectable = true;
                }                    

                if (i == 0)
                {
                    Node.NodeType = DungeonNode.Difficulty.friendly;
                }
                else
                {
                    //expand on later
                    Node.NodeType = (DungeonNode.Difficulty)Random.Range(0, 4);
                }

                Column.Add(Node);
            }
            Nodes.Add(Column);
        }

        Nodes = AddConnections(Nodes);

        return Nodes;           
    }

    public List<List<DungeonNode>> AddConnections(List<List<DungeonNode>> Dungeon)
    {
        //So messy Needs to be redone -----------------------------------------------------------------------------------------------------------------------------------------        
        //connections.Add(new int[] { i, Random.Range(1,4) });
        for (int i = 0; i < Dungeon.Count - 1; i++)
        {
            for (int j = 0;j < Dungeon[i].Count; j++)
            {
                List<Vector2> connections = new List<Vector2>();
                if (i == 0 || i == (Dungeon.Count - 2)) 
                {
                    if (i == 0)
                    {
                        connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                        connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                        connections.Add(new Vector2(Dungeon[i + 1][2].x, Dungeon[i + 1][2].y));
                    }
                    else
                    {
                       connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                    }
                } //everything above this should be gucci
                else if (i > 0 && i < Dungeon.Count - 3)
                {
                    if (Dungeon[i].Count > Dungeon[i + 1].Count) // 5 - > 4 or 4 -> 3
                    {
                        if (j == 0)
                        {
                            connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                        }
                        else if (j == Dungeon[i].Count - 1)
                        {
                            int newY = Dungeon[i + 1].Count - 1; // num y -1
                            connections.Add(new Vector2(Dungeon[i + 1][newY].x, Dungeon[i + 1][newY].y));
                        }
                        else
                        {
                            connections.Add(new Vector2(Dungeon[i + 1][j].x, Dungeon[i + 1][j].y));
                            int oldY = j - 1;
                            connections.Add(new Vector2(Dungeon[i + 1][oldY].x, Dungeon[i + 1][oldY].y));
                        }
                    }
                    else if (Dungeon[i].Count < Dungeon[i + 1].Count) // 4 - > 5 or 3 -> 4
                    {
                        connections.Add(new Vector2(Dungeon[i + 1][j].x, Dungeon[i + 1][j].y));
                        int oldY = j + 1;
                        connections.Add(new Vector2(Dungeon[i + 1][oldY].x, Dungeon[i + 1][oldY].y));
                    }
                    else // same to same
                    {
                        connections.Add(new Vector2(Dungeon[i + 1][j].x, Dungeon[i + 1][j].y));
                    }
                }
                else// to deal with edge case of potentially 5 -> 3 in the second to last column PROBLEMS
                {
                    if (Dungeon[i].Count > Dungeon[i + 1].Count)
                    {
                        int diff = Dungeon[i].Count - Dungeon[i + 1].Count;
                        if (diff == 2 && i == 5)
                        {
                            switch (j)
                            {
                                case 0:
                                    connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));                                    
                                    break;
                                case 1:
                                    connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                                    connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                                    break;
                                case 2:
                                    connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                                    break;
                                case 3:
                                    connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                                    connections.Add(new Vector2(Dungeon[i + 1][2].x, Dungeon[i + 1][2].y));
                                    break;
                                case 4:
                                    connections.Add(new Vector2(Dungeon[i + 1][2].x, Dungeon[i + 1][2].y));
                                    break;
                                default:
                                    break;
                            }
                            
                        }
                        else
                        {
                            switch (j)
                            {
                                case 0:
                                    connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                                    break;
                                case 1:
                                    connections.Add(new Vector2(Dungeon[i + 1][0].x, Dungeon[i + 1][0].y));
                                    connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                                    break;
                                case 2:
                                    connections.Add(new Vector2(Dungeon[i + 1][1].x, Dungeon[i + 1][1].y));
                                    connections.Add(new Vector2(Dungeon[i + 1][2].x, Dungeon[i + 1][2].y));
                                    break;
                                case 3:
                                    connections.Add(new Vector2(Dungeon[i + 1][2].x, Dungeon[i + 1][2].y));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        connections.Add(new Vector2(Dungeon[i + 1][j].x, Dungeon[i + 1][j].y));
                    }

                }
                Dungeon[i][j].connections = connections;

            }            

        }
        
        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------
        return Dungeon;
    }

    public void Renderlayout(List<List<DungeonNode>> Nodes)
    {
        for (int x = 0; x < Nodes.Count; x++)
        {
            for (int y = 0; y < Nodes[x].Count; y++)
            {
                int Difficulty = (int)Nodes[x][y].NodeType;
                int nodeX = (int)Nodes[x][y].x;
                int nodeY = (int)Nodes[x][y].y;//change

                int nodeRow = (int)Nodes[x][y].row;
                int nodeCol = (int)Nodes[x][y].col;//change

                bool selected = Nodes[x][y].selectable;

                var spawnedTile = Instantiate(circlePrefab, new Vector2(Nodes[x][y].x, Nodes[x][y].y), Quaternion.identity);
                spawnedTile.name = $"Tile {Nodes[x][y].x} {Nodes[x][y].y}";

                spawnedTile.Init(Difficulty, nodeX, nodeX, selected, nodeRow, nodeCol);
                
                for (int j = 0; j < Nodes[x][y].connections.Count; j++)
                {
                    GameObject Lineholder = new GameObject();
                    Lineholder.AddComponent<LineRenderer>();

                    var Line = Lineholder.GetComponent<LineRenderer>();
                    Line.positionCount = 2;
                    Line.SetPosition(0, new Vector2(Nodes[x][y].x, Nodes[x][y].y));
                    Line.SetPosition(1, (Nodes[x][y].connections[j]));
                    Line.startWidth = (float)0.35;
                    Line.endWidth = (float)0.35;
                    Line.colorGradient = _gradient;
                    Line.material = _material;
                }

            }
        }

    }

    
    public List<List<DungeonNode>> GetNodes()
    {
        return _nodes;
    }

    public void SetNodes(List<List<DungeonNode>> nodes)
    {
        _nodes = nodes;
    }

    public void NodeClicked(int row, int col)
    {
        if (currentCol + 1 == col)
        {
            DataManager.Instance.SaveDungeon(_nodes, row, col);
            //SceneManager.LoadScene("Dungeon_layout");
            SceneManager.LoadSceneAsync("Testing_Ground");
            GameManager.Instance.ChangeState(GameManager.GameState.GenerateRoom);
            //GameManager.Instance.ChangeState(GameManager.GameState.LoadDungeon);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!DataManager.Instance.HasDungeon)
        {
            GenerateDungeon();

            Renderlayout(_nodes);
        }
        

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        //var spawnedTile = Instantiate(Line);

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //pseudo code to move to game manager
        /*
        if(DungeonLayout != null)
        {
            LoadDungeonLayout();
        }
        else
        {
            GenerateDungeon();
        }  
        */

    }


}
