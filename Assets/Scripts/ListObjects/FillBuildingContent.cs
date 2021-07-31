using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FillBuildingContent : MonoBehaviour
{
    public GameObject buildingButton;

    public AllBuildings allbuildings;
    List<Building> buildingList;

    public BuildingController buildingController;
    //Fill conent area will all plants in inventory
    private void OnEnable()
    {
        //get list of plants from AllPlants script
        buildingList = allbuildings.allBuildings;

        //iterate through list
        foreach (Building building in buildingList)
        {
            //spawn buttons for each plant
            GameObject curBuildingB = Instantiate(buildingButton, Vector3.zero, Quaternion.identity);
            curBuildingB.transform.SetParent(this.transform, false);

            curBuildingB.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = building.name;

            OnBuildingButton onBuildingButton = curBuildingB.transform.GetComponent<OnBuildingButton>();
            onBuildingButton.buildingNum = building.buildingNum;
            onBuildingButton.buildingController = buildingController;
        }
    }
}
