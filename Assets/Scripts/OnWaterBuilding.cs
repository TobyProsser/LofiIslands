using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWaterBuilding : MonoBehaviour
{
    public Color waterGridColor;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GridSquare")
        {
            other.GetComponent<SpriteRenderer>().color = waterGridColor;
        }
    }
}
