using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnInventoryItems : MonoBehaviour
{
    public GameObject item;
    Vector3 spawnPos;

    AllFish allFish;
    AllPlants allPlants;

    InventoryData inventoryData;

    private void Awake()
    {
        allFish = GameObject.FindGameObjectWithTag("GameController").GetComponent<AllFish>();
        allPlants = GameObject.FindGameObjectWithTag("GameController").GetComponent<AllPlants>();

        inventoryData = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryData>();
        print(inventoryData.fishInInventory.Count);
        StartCoroutine(SpawnItems());
    }

    IEnumerator SpawnItems()
    {
        List<int> uniqueFish = inventoryData.fishInInventory.Distinct().ToList();

        foreach (int fish in uniqueFish)
        {
            spawnPos = new Vector3(Random.Range(-1.7f, 1.7f), 4, 0);
            GameObject curItem = Instantiate(item, spawnPos, Quaternion.identity);
            curItem.transform.Rotate(new Vector3(90, 0, 0));

            yield return new WaitForSeconds(.1f);
        }

        List<int> uniquePlants = inventoryData.plantsInInventory.Distinct().ToList();

        foreach (int plant in uniquePlants)
        {
            spawnPos = new Vector3(Random.Range(-1.7f, 1.7f), 4, 0);
            GameObject curItem = Instantiate(item, spawnPos, Quaternion.identity);
            curItem.transform.Rotate(new Vector3(90, 0, 0));

            yield return new WaitForSeconds(.1f);
        }
    }
}
