using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUi : MonoBehaviour
{
    //스크립트 사용 안함

    private Transform cameraToLookAt;


    void Start()
    {
        cameraToLookAt = Camera.main.transform;
    }

    void Update()
    {

        transform.LookAt(transform.position + cameraToLookAt.rotation * Vector3.back, cameraToLookAt.rotation * Vector3.down);

    }
}
