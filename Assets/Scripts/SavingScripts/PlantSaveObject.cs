using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantSaveObject
{
    [SerializeField]
    public float[] position;

    public int plantNumber;

    public int currentgrowthStep;
    public float growthTimeLeft;
}
