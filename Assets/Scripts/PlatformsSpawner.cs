using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsSpawner : MonoBehaviour
{
    [SerializeField]
    public static float platformSpeed;
    [SerializeField]
    public static float spawnSpeed;

    public List<GameObject> platforms = new List<GameObject>();

    void Awake()
    {
        platformSpeed = 1.4f;
        spawnSpeed = 16;
    }

    private void Start()
    {
        StartCoroutine(SpawnPlatforms());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnPlatforms()
    {
        Vector3 spawnLoc;

        while (true)
        {
            float zVal = Random.Range(-12, 0);
            spawnLoc = new Vector3(zVal, -9.5f, 30);

            GameObject curPlatform = Instantiate(platforms[Random.Range(0, platforms.Count)], spawnLoc, Quaternion.identity);

            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
