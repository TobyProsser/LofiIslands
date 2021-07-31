using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OnPlantButton : MonoBehaviour
{
    //Plant's number in allPlants list
    public int plantNum;

    public PlayerPlantingScript playerPlantingScript;

    void Awake()
    {
        //subscribe to the onClick event
        this.GetComponent<Button>().onClick.AddListener(CustomButton_onClick);
    }

    //Handle the onClick event
    void CustomButton_onClick()
    {
        playerPlantingScript.GetCurPlant(plantNum);
    }
}
