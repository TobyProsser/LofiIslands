using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTestPlacementObject : MonoBehaviour
{
    public Color startColor;
    public Color selectedColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GridSquare")
        {
            other.GetComponent<SpriteRenderer>().color = selectedColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GridSquare")
        {
            other.GetComponent<SpriteRenderer>().color = startColor;
        }
    }
}
