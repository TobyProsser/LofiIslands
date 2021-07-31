using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGroundCheck : MonoBehaviour
{
    int groundChecks = 0;

    private void Start()
    {
        StartCoroutine(CheckGround());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grass" || other.tag == "Stone") groundChecks++;
    }

    IEnumerator CheckGround()
    {
        yield return new WaitForSeconds(1f);
        print(groundChecks);
        //if(groundChecks < 4 && this.transform.parent != null) this.transform.parent.GetComponent<BuildingClearScript>().noGround = true;
    }
}
