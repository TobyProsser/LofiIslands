using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillPlantContent : MonoBehaviour
{
    public GameObject plantButton;

    public AllPlants allPlants;
    List<Plant> plantsList;

    public PlayerPlantingScript playerPlantingScript;
    //Fill conent area will all plants in inventory
    private void OnEnable()
    {
        //get list of plants from AllPlants script
        plantsList = allPlants.allPlants;

        //iterate through list
        foreach (Plant plant in plantsList)
        {
            //spawn buttons for each plant
            GameObject curPlantB = Instantiate(plantButton, Vector3.zero, Quaternion.identity);
            curPlantB.transform.SetParent(this.transform, false);

            curPlantB.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = plant.name;

            OnPlantButton onPlantButton = curPlantB.transform.GetComponent<OnPlantButton>();
            onPlantButton.plantNum = plant.plantNum;
            onPlantButton.playerPlantingScript = playerPlantingScript;
        }
    }
}
