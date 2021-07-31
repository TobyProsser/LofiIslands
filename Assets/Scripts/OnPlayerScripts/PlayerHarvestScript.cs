using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHarvestScript : MonoBehaviour
{
    public bool canHarvest;
    bool harvesting;

    public InventoryData inventoryData;

    List<GameObject> selectedPlants = new List<GameObject>();

    NavMeshAgent agent;
    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (canHarvest && Input.GetMouseButton(0) && Input.touchCount != 2)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //int layerMask = 11 << 8;
            //layerMask = ~layerMask;
            LayerMask mask = LayerMask.GetMask("Plant");

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                print(hit.transform.tag);
                if (hit.transform.tag == "Plant")
                {
                    //Get plant gameObject
                    GameObject curPlantObject = hit.transform.gameObject;

                    //if plant is not already in list, add it
                    if (!selectedPlants.Contains(curPlantObject)) selectedPlants.Add(curPlantObject);

                    //if not already harvesting, run coroutine,
                    //else do not run it
                    if (!harvesting)
                    {
                        StartCoroutine(WalkToAndHarvestPlant());
                        harvesting = true;
                    }
                }
            }
        }
    }

    IEnumerator WalkToAndHarvestPlant()
    {
        GameObject curPlant = selectedPlants[0];

        int i = 0;
        while (selectedPlants.Count > i)
        {
            //get current plant in list
            curPlant = selectedPlants[i];

            //Set agents destination to current plant
            agent.SetDestination(curPlant.transform.position);
            //Wait for agent to get to plant
            while (Mathf.Abs(Vector3.Distance(agent.transform.position, curPlant.transform.position)) > 1.8f)
            {
                //print(Mathf.Abs(Vector3.Distance(agent.transform.position, curPlant.transform.position)));
                yield return null;
            }

            //Get plant data from gameobject
            Plant curPlantData = curPlant.GetComponent<OnPlantScript>().thisPlantInfo;
            //If plant is fully grown, add it to inventory
            if (curPlantData.growthTime <= 0)
            {
                //save plant's number to player's inventory
                inventoryData.plantsInInventory.Add(curPlantData.plantNum);
            }

            Destroy(curPlant);

            //While there are still plants in the list, get next plant
            //or break loop
            if (selectedPlants.Count < i) break;

            i++;
            yield return null;
        }

        selectedPlants.Clear();

        harvesting = false;
        print("Plants harvested");
    }
}
