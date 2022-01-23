using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{    
    public float rotSpeed;

    float mx;
    float my;


    void Start()
    {
       
    }

   
    void Update()
    {
        if(GameManager.instance.state != GameState.Start)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            return;
        }


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        mx += h * rotSpeed * Time.deltaTime;
        my += v * rotSpeed * Time.deltaTime;

        if(my>=80)
        {
            my = 80;
        }
        else if(my<=-80)
        {
            my = -80;
        }

        if (mx >= 60)
            mx = 60;
        else if(mx <= -60)
            mx = -60;

        transform.eulerAngles = new Vector3(-my, mx, 0);
    }
}
