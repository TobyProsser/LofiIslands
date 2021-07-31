using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class InventoryData : MonoBehaviour
{
    private static InventoryData instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    //Saves items index number in list
    public List<int> fishInInventory = new List<int>();
    public List<int> plantsInInventory = new List<int>();

    private void OnEnable()
    {
        Load();
    }
    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/Inventory.dat")) file = File.Open(Application.persistentDataPath + "/Inventory.dat", FileMode.Open);
        else file = File.Create(Application.persistentDataPath + "/Inventory.dat");

        InventorySave data = new InventorySave();

        data.fishInInventory = fishInInventory;
        data.plantsInInventory = plantsInInventory;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        //File.Delete(Application.persistentDataPath + "/Inventory.dat");
        if (File.Exists(Application.persistentDataPath + "/Inventory.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Inventory.dat", FileMode.Open);
            InventorySave data = (InventorySave)bf.Deserialize(file);
            file.Close();

            fishInInventory = data.fishInInventory;
            plantsInInventory = data.plantsInInventory;
        }
    }
}

[System.Serializable]
public class InventorySave
{
    public List<int> fishInInventory;
    public List<int> plantsInInventory;
}
