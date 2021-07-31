using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class IslandSaveScript : MonoBehaviour
{
    public int islandNumber;

    public int offsetX;
    public int offsetY;

    public bool notFirstload;
    public List<SeralizableVector3> treeLocations = new List<SeralizableVector3>();

    public List<PlantSaveObject> plantsOnIsland = new List<PlantSaveObject>();
    public List<BuildingSaveObject> buildingsOnIsland = new List<BuildingSaveObject>();

    bool finishedLoading;

    private void Awake()
    {
        StartCoroutine(WaitToLoad());
    }

    //save island when it is disabled
    private void OnDisable()
    {
        Save();
    }
    public void Save()
    {
        BackUpIslandsOffset();
        //Get all plants before saving
        BackUpAllPlants();
        //Get all trees before saving
        BackUpAllTrees();
        //Get all buildings before saving
        BackUpAllBuildings();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/Island" + islandNumber + ".dat")) file = File.Open(Application.persistentDataPath + "/Island" + islandNumber + ".dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/Island" + islandNumber + ".dat");

        IslandData data = new IslandData();

        data.islandNumber = islandNumber;
        data.treeLocations = treeLocations;
        data.notFirstload = notFirstload;
        data.plantsOnIsland = plantsOnIsland;
        data.buildingsOnIsland = buildingsOnIsland;
        data.offsetX = offsetX;
        data.offsetY = offsetY;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        //File.Delete(Application.persistentDataPath + ""/Island" + islandNumber + ".dat"");
        if (File.Exists(Application.persistentDataPath + "/Island" + islandNumber + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Island" + islandNumber + ".dat", FileMode.Open);
            IslandData data = (IslandData)bf.Deserialize(file);
            file.Close();

            islandNumber = data.islandNumber;
            treeLocations = data.treeLocations;
            notFirstload = data.notFirstload;
            plantsOnIsland = data.plantsOnIsland;
            buildingsOnIsland = data.buildingsOnIsland;
            offsetX = data.offsetX;
            offsetY = data.offsetY;

            finishedLoading = true;
        }
    }

    //Look through all children of island, find plants
    //and put all essentail information into list
    public void BackUpAllPlants()
    {
        //Clear list so there aren't duplicates
        plantsOnIsland.Clear();
        //Loop through children
        foreach (Transform child in this.transform)
        {
            //If child has tag of plant
            if (child.transform.tag == "Plant")
            {
                //print(child.name + " " + child.position);
                //create new plant save object for current plant
                PlantSaveObject temp = new PlantSaveObject();
                //Save position data
                temp.position = new float[3];
                temp.position[0] = child.localPosition.x;
                temp.position[1] = child.localPosition.y;
                temp.position[2] = child.localPosition.z;

                //Get onPlantScript on current plant
                OnPlantScript tempPlantScript = child.GetComponent<OnPlantScript>();
                //get plant number from plant
                temp.plantNumber = tempPlantScript.thisPlantInfo.plantNum;
                temp.growthTimeLeft = tempPlantScript.curRemainingGrowthTime;
                temp.currentgrowthStep = tempPlantScript.currentgrowthStep;

                //Add this plant to list
                plantsOnIsland.Add(temp);
            }
        }
    }

    //Find all trees in children of island
    //Add them to the list
    public void BackUpAllTrees()
    {
        //Clear list so there aren't duplicates
        treeLocations.Clear();
        //Loop through children
        foreach (Transform child in this.transform)
        {
            //If child has tag of plant
            if (child.transform.tag == "Tree")
            {
                //Create new serializable vector3
                SeralizableVector3 temp = new SeralizableVector3();
                //save position
                temp.x = child.transform.localPosition.x;
                temp.y = child.transform.localPosition.y;
                temp.z = child.transform.localPosition.z;

                //Add this tree to list
                treeLocations.Add(temp);
            }
        }
    }

    //Look through all children of island, find buildings
    //and put all essentail information into list
    public void BackUpAllBuildings()
    {
        buildingsOnIsland = new List<BuildingSaveObject>();
        //Clear list so there aren't duplicates
        buildingsOnIsland.Clear();
        //Loop through children
        foreach (Transform child in this.transform)
        {
            //If child is on building layer
            if (child.gameObject.layer == 11)
            {
                //print(child.name + " " + child.position);
                //create new plant save object for current plant
                BuildingSaveObject temp = new BuildingSaveObject();
                //Save position data
                temp.position = new float[3];
                temp.position[0] = child.localPosition.x;
                temp.position[1] = child.localPosition.y;
                temp.position[2] = child.localPosition.z;

                //Get onBuildingScript on current building
                OnBuildingScript tempbuildingScript = child.GetComponent<OnBuildingScript>();
                //get building number from plant
                temp.buildingNumber = tempbuildingScript.thisBuildingInfo.buildingNum;

                //Add this building to list
                buildingsOnIsland.Add(temp);
            }
        }
    }

    void BackUpIslandsOffset()
    {
        offsetX = this.GetComponent<SpawnIslandGenerator>().offset.x;
        offsetY = this.GetComponent<SpawnIslandGenerator>().offset.y;
    }

    //Wait for island to be given it's number before loading information
    //then generate island
    IEnumerator WaitToLoad()
    {
        //wait for island to be given number
        while (islandNumber == -1)
        {
            if (islandNumber != -1) break;
            yield return null;
        }
        //load correct island given save number
        finishedLoading = false;
        Load();

        while (!finishedLoading)
        {
            if (finishedLoading) break;
            yield return null;
        }

        if (finishedLoading)
        {
            //If this isn't the first time the island is being loaded,
            //give the island the saved offset
            //otherwise let the island generated with given offset so
            //Correct offset is then saved
            if (notFirstload)
            {
                //Give island generator correct offset information
                this.GetComponent<SpawnIslandGenerator>().offset.x = offsetX;
                this.GetComponent<SpawnIslandGenerator>().offset.y = offsetY;
            }
            //Generate island
            this.GetComponent<SpawnIslandGenerator>().GenerateIsland();
        }
    }

    [ContextMenu("DeleteSaveFile")]
    public void DeleteSaveFile()
    {
        File.Delete(Application.persistentDataPath + "/Island" + islandNumber + ".dat");
    }
}
[System.Serializable]
public class IslandData
{
    public int islandNumber;
    public int offsetX;
    public int offsetY;
    public bool notFirstload;
    public List<SeralizableVector3> treeLocations;
    public List<PlantSaveObject> plantsOnIsland;
    public List<BuildingSaveObject> buildingsOnIsland;
}
