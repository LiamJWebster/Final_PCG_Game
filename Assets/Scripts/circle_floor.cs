using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circle_floor : MonoBehaviour
{
    [SerializeField] private Color EasyColour;
    [SerializeField] private Color NormalColour;
    [SerializeField] private Color AbadonedColour;
    [SerializeField] private Color DangerousColour;

    [SerializeField] private SpriteRenderer renderer;

    [SerializeField] private GameObject highlight;

    public void Init(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                renderer.color = EasyColour;
                break;
            case 1:
                renderer.color = NormalColour;
                break;
            case 2:
                renderer.color = DangerousColour;
                break;
            case 3:
                renderer.color = AbadonedColour;
                break;
        }
    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
