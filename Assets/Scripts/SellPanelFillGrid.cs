using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SellPanelFillGrid : MonoBehaviour
{
    public GameObject itemButton;

    AllFish allFish;
    AllPlants allPlants;

    InventoryData inventoryData;

    private void Awake()
    {
        allFish = GameObject.FindGameObjectWithTag("GameController").GetComponent<AllFish>();
        allPlants = GameObject.FindGameObjectWithTag("GameController").GetComponent<AllPlants>();

        inventoryData = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryData>();
    }

    private void OnEnable()
    {
        SpawnItemButtons();
    }

    void SpawnItemButtons()
    {
        //get list of fish in inventory
        List<int> sortedFish = inventoryData.fishInInventory;
        //sort the list
        sortedFish.Sort();
        print("Size: " + sortedFish.Count);
        //loop through List
        for (int i = 0; i < sortedFish.Count; i++)
        {
            GameObject curItem = Instantiate(itemButton, Vector3.zero, Quaternion.identity);
            curItem.transform.SetParent(this.transform, false);

            //Get name of item using index number in allfish list and set it to button text
            curItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = allFish.allFish[sortedFish[i]].name;
        }

        //get list of fish in inventory
        List<int> sortedPlants = inventoryData.plantsInInventory;
        //sort the list
        sortedPlants.Sort();
        print("Size: " + sortedPlants.Count);
        //loop through List
        for (int i = 0; i < sortedPlants.Count; i++)
        {
            GameObject curItem = Instantiate(itemButton, Vector3.zero, Quaternion.identity);
            curItem.transform.SetParent(this.transform, false);

            //Get name of item using index number in allfish list and set it to button text
            curItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = allPlants.allPlants[sortedPlants[i]].name;
        }
    }
}
