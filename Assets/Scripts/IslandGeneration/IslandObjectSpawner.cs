using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandObjectSpawner : MonoBehaviour
{
    int chestsAmount = 1;
    public GameObject chest;
    public GameObject testCube;

    public bool spawnBuildings;
    public int buildingsAmount = 1;
    public GameObject building;

    int buildingTryAmount = 0;

    public GameObject tree;
    public int treesAmount = 50;  //Higher means less amount of trees
    List<SeralizableVector3> treeLocations = new List<SeralizableVector3>();

    public GameObject grass;
    public GameObject tallGrass;

    List<Transform> spawnLocations = new List<Transform>();

    [HideInInspector]
    public List<GameObject> spawnedEmptyBuildings = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> spawnBuildingList = new List<GameObject>();
    bool buildingsSpawned;

    IslandSaveScript islandSaveScript;

    AllPlants allPlantsList;
    AllBuildings allBuildingsList;

    public GameObject gridSquare;

    GameObject gameController;

    public void IslandSpawned(List<Transform> spawnLocs)
    {
        //Find Lists
        gameController = GameObject.FindGameObjectsWithTag("GameController")[0];

        allPlantsList = gameController.GetComponent<AllPlants>();
        allBuildingsList = gameController.GetComponent<AllBuildings>();

        islandSaveScript = this.GetComponent<IslandSaveScript>();

        //If this is the first time the island is being loaded
        //Spawn new objects
        //Otherwise spawn saved locations

        spawnLocations = spawnLocs;

        if (!islandSaveScript.notFirstload)
        {
            SpawnNewChests();
            SpawnNewTrees();
            SpawnNewGrass();
            //LookForBuildingLocations(4, 6);

            islandSaveScript.notFirstload = true;
        }
        else
        {
            //get tree locations from save script
            treeLocations = islandSaveScript.treeLocations;
            SpawnTrees();
            SpawnNewGrass();
            SpawnPlants();
            SpawnBuidlings();
        }

        spawnGrid();

        //Saved spawned objects to IslandSaveScript
        islandSaveScript.Save();
    }

    void SpawnNewChests()
    {
        for (int i = 0; i < chestsAmount; i++)
        {
            GameObject curChest = Instantiate(chest);
            curChest.transform.parent = this.transform;
            Transform spawnCubeTrans = FindChestSpawnLoc();
            Vector3 spawnPos = spawnCubeTrans.localPosition + new Vector3(.5f, .5f + (spawnCubeTrans.localScale.y / 2), -1f);
            curChest.transform.localPosition = spawnPos;
        }
    }

    Transform FindChestSpawnLoc()
    {
        int start = Random.Range(0, spawnLocations.Count -1);
        Transform trans = null;

        for (int i = start; i < spawnLocations.Count -1; i++)
        {
            if (spawnLocations[i].position.y == spawnLocations[i + 1].position.y)
            {
                trans = spawnLocations[i];
                break;
            }

            print(spawnLocations[i].position.y + " " + spawnLocations[i + 1].position.y);
        }

        if (trans == null)
        {
            for (int i = 0; i < spawnLocations.Count -1; i++)
            {
                if (spawnLocations[i].position.y == spawnLocations[i + 1].position.y)
                {
                    trans = spawnLocations[i];
                    break;
                }
            }
        }

        return trans;
    }

    void SpawnNewTrees()
    {
        for (int i = 0; i < spawnLocations.Count - 1; i++)
        {
            int treeChance = Random.Range(0, treesAmount);
            if (treeChance == 0)
            {
                GameObject curTree = GameObject.Instantiate(tree);
                curTree.transform.position = new Vector3(spawnLocations[i].position.x, (spawnLocations[i].localScale.y - 4.7f), spawnLocations[i].position.z);
                curTree.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));

                curTree.transform.parent = this.transform;

                //add location to list
                SeralizableVector3 temp = new SeralizableVector3();
                temp.x = curTree.transform.localPosition.x;
                temp.y = curTree.transform.localPosition.y;
                temp.z = curTree.transform.localPosition.z;

                treeLocations.Add(temp);
            }
        }
        //send tree locations to islandSaveScript
        islandSaveScript.treeLocations = treeLocations;
    }
    //Spawn all trees using list in island's saved data
    void SpawnTrees()
    {
        //Loop through saved list
        for (int i = 0; i < treeLocations.Count; i++)
        {
            //Instantiate tree
            GameObject curTree = GameObject.Instantiate(tree);
            //Set its parent to island
            curTree.transform.parent = this.transform;
            //set its position to saved poition
            curTree.transform.localPosition = new Vector3(treeLocations[i].x, treeLocations[i].y, treeLocations[i].z);
            //Randomly rotate it
            curTree.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 5), 0));
        }
    }
    //Spawn all plants using list in island's saved data
    void SpawnPlants()
    {
        List<PlantSaveObject> plantsOnIsland = islandSaveScript.plantsOnIsland;

        for (int i = 0; i < plantsOnIsland.Count; i++)
        {
            //Get current plants saved information from saved list
            PlantSaveObject curPlantSave = plantsOnIsland[i];
            //Get plants index number
            int curPlantNumber = curPlantSave.plantNumber;
            //Get plants 'mesh' using its index number in the plants list
            GameObject plantObject = allPlantsList.allPlants[curPlantNumber].plantObjects[curPlantSave.currentgrowthStep];
            //Spawn plant
            GameObject curPlant = Instantiate(plantObject);
            //Set its parent to island
            curPlant.transform.parent = this.transform;
            //Set plants local position
            curPlant.transform.localPosition = new Vector3(curPlantSave.position[0], curPlantSave.position[1], curPlantSave.position[2]);

            curPlant.AddComponent<OnPlantScript>();
            curPlant.GetComponent<OnPlantScript>().thisPlantInfo = allPlantsList.allPlants[curPlantNumber];

            curPlant.AddComponent<BoxCollider>();
            curPlant.GetComponent<BoxCollider>().isTrigger = true;

            curPlant.transform.tag = "Plant";
            curPlant.layer = 9;
        }
    }

    void SpawnBuidlings()
    {
        if (islandSaveScript.buildingsOnIsland != null)
        {
            List<BuildingSaveObject> buildingsOnIsland = islandSaveScript.buildingsOnIsland;

            for (int i = 0; i < buildingsOnIsland.Count; i++)
            {
                //Get current Buildings saved information from saved list
                BuildingSaveObject curBuildingSave = buildingsOnIsland[i];
                //Get Buildings index number
                int curBuildingNumber = curBuildingSave.buildingNumber;
                //Get Buildings 'mesh' using its index number in the Buildings list
                GameObject buildingObject = allBuildingsList.allBuildings[curBuildingNumber].buildingObject;
                //Spawn Building
                GameObject curBuilding = Instantiate(buildingObject);
                //Set its parent to island
                curBuilding.transform.parent = this.transform;
                //Set buildings local position
                curBuilding.transform.localPosition = new Vector3(curBuildingSave.position[0], curBuildingSave.position[1], curBuildingSave.position[2]);
            }
        }
    }

    void SpawnNewGrass()
    {
        GameObject grassHolder = new GameObject();
        grassHolder.name = "GrassHolder";
        grassHolder.transform.parent = this.transform;

        for (int i = 0; i < spawnLocations.Count; i++)
        {
            int grassChance = Random.Range(0, 2);

            if (grassChance == 0)
            {
                int tallChance = Random.Range(0, 10);

                Vector3 spawnLoc = spawnLocations[i].position + new Vector3(0, spawnLocations[i].transform.localScale.y/2, 0);

                if (tallChance != 0)
                {
                    GameObject curGrass = Instantiate(grass, spawnLoc, Quaternion.identity);
                    curGrass.transform.SetParent(grassHolder.transform);
                }
                else
                {
                    GameObject curGrass = Instantiate(tallGrass, spawnLoc, Quaternion.identity);
                    curGrass.transform.SetParent(grassHolder.transform);
                }
            }
        }
    }

    public void spawnGrid()
    {
        GameObject gridObject = new GameObject();
        gridObject.transform.parent = this.transform;
        gridObject.name = "gridObject";
        gridObject.tag = "GridObject";

        foreach (Transform trans in spawnLocations)
        {
            GameObject curGridSquare = Instantiate(gridSquare, trans.position + new Vector3(0, trans.lossyScale.y * .5f + .01f, 0), Quaternion.identity);
            curGridSquare.transform.Rotate(90, 0, 0);
            curGridSquare.transform.parent = gridObject.transform;
        }

        gridObject.SetActive(false);
    }

    /*
        //Loop through each location and draw box the size of the building on top
        //of it. If the box doesnt intercept with anything, spawn building on that
        //location
        void LookForBuildingLocations(int sizeX, int sizeZ)
        {
            for (int i = 0; i < 1; i++)
            {
                bool locationFound = false;
                Transform buildingTrans = null;

                print(spawnLocations.Count);
                foreach (Transform pos in spawnLocations)
                {
                    Collider[] hitColliders = Physics.OverlapBox(pos.position + new Vector3(0, (pos.lossyScale.y / 2) + .5f, 0), new Vector3(sizeX/2, .5f, sizeZ/2), Quaternion.identity);

                    //GameObject test = Instantiate(testCube, pos.position + new Vector3(0, (pos.lossyScale.y/2) + 1, 0), Quaternion.identity);
                    //test.transform.localScale = new Vector3(sizeX / 2, .5f, sizeZ / 2);

                    if (hitColliders.Length == 0)
                    {
                        if (CheckBuildingGround(pos, sizeX, sizeZ))
                        {
                            locationFound = true;
                            buildingTrans = pos;
                            break;
                        }
                    }
                }

                if (locationFound)
                {
                    print("LocationFound " + buildingTrans.position);
                    GameObject curHouse = Instantiate(building, buildingTrans.position + new Vector3(0, (buildingTrans.lossyScale.y / 2) + .5f, 0), Quaternion.identity);
                    curHouse.transform.localScale = new Vector3(sizeX / 2, 1, sizeZ / 2);
                }
            }
        }

        bool CheckBuildingGround(Transform trans, int x, int z)
        {
            //place collider at top right corner of building position to
            //Check if there is ground
            Collider[] hitColliders = Physics.OverlapBox(trans.position + new Vector3(x/ 2 - .5f, (-trans.lossyScale.y / 2) + .5f, z/ 2 - .5f), Vector3.one, Quaternion.identity);
            //if there is ground in that location
            //check next location
            if (hitColliders.Length > 1)
            {
                GameObject test = Instantiate(testCube, (trans.position + new Vector3(x / 2 - .5f, (-trans.lossyScale.y / 2) + .5f, z / 2 - .5f)), Quaternion.identity);
                test.name = "TopLeft";

                //place collider at bottom left corner of building position to
                //Check if there is ground
                Collider[] hitColliders1 = Physics.OverlapBox(trans.position + new Vector3(-x / 2 + .5f, (-trans.lossyScale.y / 2) + .5f, -z / 2 + .5f), Vector3.one, Quaternion.identity);

                //if there is also ground in this location
                //return true
                if (hitColliders1.Length > 1)
                {
                    test = Instantiate(testCube, trans.position + new Vector3(-x / 2 + .5f, (-trans.lossyScale.y / 2) + .5f, -z / 2 + .5f), Quaternion.identity);
                    test.name = "BottomRight";

                    return true;
                }
                else
                {
                    Destroy(test);
                    return false;
                }
            }
            else return false;
        }

    */
}
