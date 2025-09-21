using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dont_Destroy : MonoBehaviour
{
    public static Dont_Destroy Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("cum");
            Destroy(this.gameObject);
        }

        

        DontDestroyOnLoad(Instance);
    }
}
