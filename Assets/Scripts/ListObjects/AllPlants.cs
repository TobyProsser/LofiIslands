using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlants : MonoBehaviour
{
    [SerializeField]
    public List<Plant> allPlants;

    private static AllPlants instance;

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
}
