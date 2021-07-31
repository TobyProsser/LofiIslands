using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingClearScript : MonoBehaviour
{
    public IslandObjectSpawner islandObjectSpawner;

    bool removeSurroundings = false;
    bool incorrectPos = false;

    private void Start()
    {
        StartCoroutine(RemoveComponents());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Stone" && !incorrectPos)
        {
            IncorrectPos();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Chest" || other.tag == "EnemySpawner" || other.tag == "Building")
        {
            if (!incorrectPos) IncorrectPos();
        }

        if (!incorrectPos && removeSurroundings)
        {
            if (other.tag == "Tree") Destroy(other.gameObject);
        }
    }

    IEnumerator RemoveComponents()
    {
        yield return new WaitForSeconds(1.5f);
        removeSurroundings = true;
        yield return new WaitForSeconds(.5f);
        if (this.gameObject.transform != null)
        {
            Destroy(this.GetComponent<Rigidbody>());

            islandObjectSpawner.spawnedEmptyBuildings.Add(this.gameObject);

            if(this.transform != null) Destroy(this);
        }
    }

    void IncorrectPos()
    {
        incorrectPos = true;
        //if (this.transform != null) islandObjectSpawner.SpawnBuilding();
        Destroy(this.gameObject);
    }
}
