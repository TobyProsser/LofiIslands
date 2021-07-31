using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *This script holds each unique plant's information and is used to display
 *all the plants in the UI
*/
//needs to be serializable to set information in inspector
[System.Serializable]
public class Plant
{   
    //Plants number in list
    public int plantNum;

    public string name;

    public float cost;
    public float sellPrice;

    public int growthSteps;
    public float growthTime;

    public GameObject[] plantObjects;
}
