using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerPlantingScript : MonoBehaviour
{
    public bool canPlant;
    bool planting;

    public GameObject tempPlacementObject;

    public GameObject plantsPanel;
    public AllPlants allPlants;
    Plant curPlant;

    public GameObject farmLandCube;

    List<Vector3> selectedLocations = new List<Vector3>();

    public GameObject camera;

    NavMeshAgent agent;

    public Color gridStartColor;
    public Color gridSelectedColor;

    private void Awake()
    {
        plantsPanel.SetActive(false);

        agent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (canPlant)
        {
            //If plsyer clicks on cube and mouse is not over UI
            //add cube to a list
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && Input.touchCount != 2) AddCubesToList();
        }
    }

    void AddCubesToList()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = 11 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            print(hit.transform.tag);
            if (hit.transform.tag == "GridSquare")
            {
                print("CLicked");
                //place temporary object on clicked area
                //Takes the loctation player has clicked on, always rounds down to closest int
                //The adjusts position to center object
                //This accounts for the player clicking inside anyone in a 'grid' space
                //and placing the object in the correct location
                Vector3 location = new Vector3(Mathf.Floor(hit.point.x) + 0.5f, Mathf.Floor(hit.point.y) + 0.5f, Mathf.Floor(hit.point.z) + 0.5f);
                GameObject curGridSquare = hit.transform.gameObject;

                //SpriteRenderer curRenderer = curGridSquare.GetComponent<SpriteRenderer>();

                //curRenderer.color = gridSelectedColor;

                StartCoroutine(ChangeGridColor(curGridSquare, gridStartColor, gridSelectedColor));

                selectedLocations.Add(location);
            }
        }
    }

    public void PlantButton()
    {
        planting = true;
        StartCoroutine(TurnToFarmLand());
    }

    //Turn all cubes in list to current selected Plant
    IEnumerator TurnToFarmLand()
    {
        //get current island
        GameObject curIsland = ChangingIslandsController.currentIsland;

        //Remove duplicates in list
        List<Vector3> removeDups = selectedLocations.Distinct().ToList();

        //Loop through list
        int i = 0;
        while (removeDups.Count > 0)
        {
            //Get current location in list
            Vector3 currentLoc = removeDups[i];
            //Set agents destination to current tree
            agent.SetDestination(currentLoc);
            //Wait for agent to get to plant
            while (Mathf.Abs(Vector3.Distance(agent.transform.position, currentLoc)) > 1.8f)
            {
                //print(Mathf.Abs(Vector3.Distance(agent.transform.position, currentPlant.transform.position)));
                yield return null;
            }
            //Remove plant from list and plant curplant
            removeDups.RemoveAt(0);

            //Spawn plant at land point
            GameObject curLand = Instantiate(curPlant.plantObjects[0]);
            curLand.transform.position = currentLoc - new Vector3(0, 0.5f, 0);
            //set plant's parent to island
            curLand.transform.parent = curIsland.transform;
            curLand.transform.tag = "Plant";
            curLand.layer = 9;

            //Give plant correct information
            curLand.AddComponent<OnPlantScript>();
            curLand.GetComponent<OnPlantScript>().thisPlantInfo = curPlant;

            curLand.AddComponent<BoxCollider>();
            curLand.GetComponent<BoxCollider>().isTrigger = true;

            //While there are still trees in the list, get next tree
            //or break loop
            if (removeDups.Count > 0)
            {
                //Check if tree still exists
                if (removeDups[0] != null)
                {
                    currentLoc = removeDups[0];
                }
                else break;
            }
            else break;

            yield return null;
        }

        //after all plants are placed, clear the list
        selectedLocations.Clear();

        planting = false;
        //Save plant information;
        curIsland.GetComponent<IslandSaveScript>().Save();
    }

    public void GetCurPlant(int plantNum)
    {
        curPlant = allPlants.allPlants[plantNum];
    }

    IEnumerator ChangeGridColor(GameObject curGridSquare, Color startCol, Color endCol)
    {
        SpriteRenderer curRenderer = curGridSquare.GetComponent<SpriteRenderer>();

        Color lerpedColor = startCol;

        float t = 100;

        while (lerpedColor != endCol)
        {
            lerpedColor = Color.Lerp(startCol, endCol, t);

            t -= Time.deltaTime / 10;

            curRenderer.color = lerpedColor;

            yield return null;
        }
    }
}
