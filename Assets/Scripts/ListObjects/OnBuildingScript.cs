using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBuildingScript : MonoBehaviour
{
    [HideInInspector]
    public Building thisBuildingInfo;

    public int buildingNum;
    AllBuildings allBuildings;

    private void Awake()
    {
        allBuildings = GameObject.FindGameObjectWithTag("GameController").GetComponent<AllBuildings>();
        thisBuildingInfo = allBuildings.allBuildings[buildingNum];
    }
}
