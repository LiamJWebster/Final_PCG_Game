using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            //print("up arrow key is held down");
           // GameObject[] gos = GameObject.FindGameObjectsWithTag(Terrain);
           // foreach (GameObject go in gos)
          //  Destroy(go);
        }

    }
}

/*
// Start is called before the first frame update
void Start()
{
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    autoCast = GetComponent<AutoCast>();
}

// Update is called once per frame
void Update()
{
    // Horizontal movement
    horizontalInput = Input.GetAxis("Horizontal");
    verticalInput = Input.GetAxis("Vertical");
    Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * moveSpeed;

    // If no input is detected, set the velocity to zero
    if (movement.magnitude == 0)
    {
        rb.velocity = Vector2.zero;
    }
    else
    {
        // Normalize the movement vector and apply move speed
        movement = movement.normalized * moveSpeed;
        rb.velocity = movement;
    }

    ManageAnimations();
}
*/