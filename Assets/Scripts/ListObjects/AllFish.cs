using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllFish : MonoBehaviour
{
    [SerializeField]
    public List<Fish> allFish;

    private static AllFish instance;

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
