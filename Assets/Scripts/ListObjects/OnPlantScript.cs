using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlantScript : MonoBehaviour
{
    //[HideInInspector]
    public Plant thisPlantInfo;

    //[HideInInspector]
    public float curRemainingGrowthTime;
    [HideInInspector]
    public int currentgrowthStep = 0;

    bool watered;
    private void Awake()
    {
        //Destroy grass covering object
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, Vector3.one, Quaternion.identity);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.tag == "ShortGrass") Destroy(collider.gameObject);
        }

        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        yield return new WaitForSeconds(.01f);

        this.GetComponent<MeshFilter>().sharedMesh = thisPlantInfo.plantObjects[currentgrowthStep].GetComponent<MeshFilter>().sharedMesh;

        curRemainingGrowthTime = thisPlantInfo.growthTime;

        float growthStep = curRemainingGrowthTime / (thisPlantInfo.growthSteps + 1);
        while (curRemainingGrowthTime > 0)
        {
            if (watered)
            {
                if (curRemainingGrowthTime % growthStep == 0)
                {
                    currentgrowthStep++;
                    this.GetComponent<MeshFilter>().sharedMesh = thisPlantInfo.plantObjects[currentgrowthStep].GetComponent<MeshFilter>().sharedMesh;
                }

                curRemainingGrowthTime -= 1;
            }

            yield return new WaitForSeconds(2);
        }

        if(curRemainingGrowthTime <= 0) this.GetComponent<MeshFilter>().sharedMesh = thisPlantInfo.plantObjects[thisPlantInfo.plantObjects.Length-1].GetComponent<MeshFilter>().sharedMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Well")
        {
            watered = true;
        }
    }
}
