using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBuildingButton : MonoBehaviour
{
    //Buildings's number in allBuildings list
    public int buildingNum;

    public BuildingController buildingController;

    void Awake()
    {
        //subscribe to the onClick event
        this.GetComponent<Button>().onClick.AddListener(CustomButton_onClick);
    }

    //Handle the onClick event
    void CustomButton_onClick()
    {
        buildingController.GetCurBuilding(buildingNum);
    }
}
