using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class ChangingIslandsController : MonoBehaviour
{
    Vector3 baseIslandLocation = new Vector3(3.5f, 0, 3.5f);
    Vector3 EndOffScreenLoc = new Vector3(3.5f, 0, 100f);
    Vector3 StartOffScreenLoc = new Vector3(3.5f, 0, -100f);

    public float islandMoveSpeed;
    [HideInInspector]
    public static GameObject currentIsland;
    [HideInInspector]
    public GameObject nextIsland;

    public GameObject baseIsland;
    public GameObject storeIsland;

    public GameObject islandButton;
    public GameObject islandButtonContentPanel;

    AllIslandsSave allIslandsSave;

    public NavMeshSurface surface;

    private void Awake()
    {
        allIslandsSave = this.GetComponent<AllIslandsSave>();
        currentIsland = GameObject.FindGameObjectsWithTag("Island")[0];

        StartCoroutine(FirstNavSurfaceReload());
    }

    public void ChangeIsland(int islandNumber)
    {
        SpawnNextIsland(islandNumber);
        //Move Islands
        StartCoroutine(MoveIslands());
    }

    IEnumerator MoveIslands()
    {
        while (true)
        {
            currentIsland.transform.position = Vector3.Lerp(currentIsland.transform.position, EndOffScreenLoc, Time.deltaTime * islandMoveSpeed);
            nextIsland.transform.position = Vector3.Lerp(nextIsland.transform.position, baseIslandLocation, Time.deltaTime * islandMoveSpeed);

            if (Mathf.Abs(Vector3.Distance(nextIsland.transform.position, baseIslandLocation)) < .1f)
            {
                nextIsland.transform.position = baseIslandLocation;
                break;
            }
            yield return null;
        }

        Destroy(currentIsland);

        currentIsland = nextIsland;

        RebuildNavSurface();
    }

    //Spawn base island, then enter its number into it's save script
    //to load correct information
    void SpawnNextIsland(int number)
    {
        //if number = 0 then spawn the village island
        //otherwise spawn given island
        if (number == 0)
        {
            nextIsland = Instantiate(storeIsland, StartOffScreenLoc, Quaternion.identity);
        }
        else
        {
            //Spawn base island
            nextIsland = Instantiate(baseIsland, StartOffScreenLoc, Quaternion.identity);

            //Give island's save script correct number so it can load it's information
            //This will also make script load and generate island
            nextIsland.GetComponent<IslandSaveScript>().islandNumber = number;
        }
    }

    public void CreateNewIsland()
    {
        //get last number in islands list and add one to it to get next island number
        int islandNumber = allIslandsSave.islands[allIslandsSave.islands.Count - 1] + 1;
        //Add new island number to allislands list
        allIslandsSave.islands.Add(islandNumber);
        //Spawn Island
        nextIsland = Instantiate(baseIsland, StartOffScreenLoc, Quaternion.identity);
        //Give island its number
        //This will also make script load and generate island
        nextIsland.GetComponent<IslandSaveScript>().islandNumber = islandNumber;

        nextIsland.GetComponent<SpawnIslandGenerator>().offset = new Vector2Int(Random.Range(100, 100000), Random.Range(100, 100000));

        //Spawn new button
        GameObject curButton = Instantiate(islandButton, Vector3.zero, Quaternion.identity);

        curButton.transform.SetParent(islandButtonContentPanel.transform, false);

        curButton.GetComponent<OnIslandButton>().islandNumber = allIslandsSave.islands[islandNumber];
        curButton.GetComponent<OnIslandButton>().changingIslandsController = this;

        curButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = allIslandsSave.islands[islandNumber].ToString();

        //Destroy the island that was just created
        //It will be respawned when button is clicked
        Destroy(nextIsland);
    }

    //ran by allIslandsSave
    public void SpawnIslandButtons(AllIslandsSave islandsSave)
    {
        //Don't respawn buttons that have already been spawned
        int numButtonsAlreadySpawned = islandButtonContentPanel.transform.childCount;
        for (int i = numButtonsAlreadySpawned; i < islandsSave.islands.Count; i++)
        {
            GameObject curButton = Instantiate(islandButton, Vector3.zero, Quaternion.identity);

            curButton.transform.SetParent(islandButtonContentPanel.transform, false);

            curButton.GetComponent<OnIslandButton>().islandNumber = islandsSave.islands[i];
            curButton.GetComponent<OnIslandButton>().changingIslandsController = this;

            curButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = islandsSave.islands[i].ToString();
        }
    }

    public void RebuildNavSurface()
    {
        surface.RemoveData();
        surface.BuildNavMesh();
    }

    IEnumerator FirstNavSurfaceReload()
    {
        yield return new WaitForSeconds(2);
        RebuildNavSurface();
    }
}
