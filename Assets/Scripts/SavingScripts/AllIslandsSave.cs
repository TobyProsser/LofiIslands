using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AllIslandsSave : MonoBehaviour
{
    //Stores numbers of all created islands which can be used to
    //load islands through their save file using the islands number
    //Island 0 is the village, island 1 is the first island
    public List<int> islands;
    public bool notFirstload;

    ChangingIslandsController changingIslands;

    void Awake()
    {
        Load();

        changingIslands = this.GetComponent<ChangingIslandsController>();
        changingIslands.SpawnIslandButtons(this);
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/AllIslands.dat")) file = File.Open(Application.persistentDataPath + "/AllIslands.dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/AllIslands.dat");

        AllIslands data = new AllIslands();

        data.islands = islands;
        data.notFirstload = notFirstload;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        //File.Delete(Application.persistentDataPath + ""/Island" + islandNumber + ".dat"");
        if (File.Exists(Application.persistentDataPath + "/AllIslands.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/AllIslands.dat", FileMode.Open);
            AllIslands data = (AllIslands)bf.Deserialize(file);
            file.Close();

            islands = data.islands;
            notFirstload = data.notFirstload;
        }

        print(notFirstload);
        //if first time saving, save the base two islands to list 
        if (!notFirstload)
        {
            islands = new List<int> { 0, 1 };
            notFirstload = true;

            Save();
        }
    }
}

[System.Serializable]
public class AllIslands
{
    public List<int> islands;
    public bool notFirstload;
}
