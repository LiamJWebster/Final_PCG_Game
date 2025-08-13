using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    private float totalRun = 1.0f;
    float mainSpeed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Keyboard commands
        //float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;
            
            transform.Translate(p);
        }

    }

    private Vector3 GetBaseInput()
    { 
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}
/*
if (Input.GetMouseButton(2)) {
lastMouse = Input.mousePosition - lastMouse;
lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
transform.eulerAngles = lastMouse;
            //Mouse  camera angle done.  
        }
        lastMouse = Input.mousePosition;
*/