using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodsInfo : MonoBehaviour
{
    public bool isEmpty = false;

    public float maxFoodTimer;
    public float curFoodTimer;

    public float posY;

    public Image foodTimerImage;

    public Mesh[] curMesh;
    public Material[] curMaterial;


    [SerializeField]
    private float divideNum;

    public bool otherNum = false;
    private void Awake()
    {
        divideNum = maxFoodTimer / 3f;

    }

    private void Start()
    {

    }
    private void Update()
    {

        if (divideNum > curFoodTimer && curFoodTimer >= 0)
        {
            if (otherNum)
            {
                GetComponent<MeshRenderer>().materials[2] = curMaterial[0];

            }
            else
            {
                GetComponent<MeshRenderer>().materials[0] = curMaterial[0];
            }
            GetComponent<MeshFilter>().mesh = curMesh[0];

        }
        else if (divideNum * 2 > curFoodTimer && curFoodTimer >= divideNum)
        {
            if (otherNum)
            {
                GetComponent<MeshRenderer>().materials[2] = curMaterial[1];

            }
            else
            {
                GetComponent<MeshRenderer>().materials[0] = curMaterial[1];
            }
            GetComponent<MeshFilter>().mesh = curMesh[1];

        }
        else if (divideNum * 3 > curFoodTimer && curFoodTimer >= divideNum * 2)
        {
            if (otherNum)
            {
                GetComponent<MeshRenderer>().materials[2] = curMaterial[2];

            }
            else
            {
                GetComponent<MeshRenderer>().materials[0] = curMaterial[2];
            }
            GetComponent<MeshFilter>().mesh = curMesh[2];

        }
        else if (divideNum * 4 > curFoodTimer && curFoodTimer >= divideNum * 3)
        {
            if (otherNum)
            {
                GetComponent<MeshRenderer>().materials[2] = null;

            }
            else
            {
                GetComponent<MeshRenderer>().materials[0] = null;
            }
            GetComponent<MeshFilter>().mesh = null;
            foodTimerImage.enabled = true;
        }


    }
}
