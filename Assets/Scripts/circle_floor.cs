using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dungeon_generation;

public class circle_floor : MonoBehaviour
{
    [SerializeField] private Color EasyColour;
    [SerializeField] private Color NormalColour;
    [SerializeField] private Color AbadonedColour;
    [SerializeField] private Color DangerousColour;

    [SerializeField] private SpriteRenderer Renderer;

    [SerializeField] private GameObject highlight;

    [SerializeField] private int x;
    [SerializeField] private int y;

    [SerializeField] private int row;
    [SerializeField] private int col;

    [SerializeField] private bool selected;
    [SerializeField] private bool selectable;


    public void Init(int difficulty, int x, int y, bool selectable, int row , int col)
    {
        this.x = x;
        this.y = y;

        this.row = row;
        this.col = col;

        this.selectable = selectable;

        if (selectable)
        {
            selected = false;   
        }
        else
        {
            selected = true;
            highlight.SetActive(true);
        }
        switch (difficulty)
        {
            case 0:
                Renderer.color = EasyColour;
                break;
            case 1:
                Renderer.color = NormalColour;
                break;
            case 2:
                Renderer.color = AbadonedColour;
                break;
            case 3:
                Renderer.color = DangerousColour;
                break;
        }
    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        if (!selected)
        {
            highlight.SetActive(false);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        if (selectable)
        {
            selected = !selected;
        }

        Dungeon_generation.Instance.NodeClicked(row, col);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
