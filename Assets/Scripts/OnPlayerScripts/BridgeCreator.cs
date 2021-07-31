using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BridgeCreator : MonoBehaviour
{
    [HideInInspector]
    public bool canBuild;

    public float maxBridgeLength;
    public GameObject bridgeCube;
    public Color startColor;
    public Color selectedColor;

    GameObject startSquare;
    GameObject endSquare;

    bool started;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && canBuild && Input.touchCount != 2)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "GridSquare")
                {
                    //If first click, set clicked square to first position
                    //else set clicked square to second position
                    if (!started)
                    {
                        if(startSquare != null) startSquare.transform.GetComponent<SpriteRenderer>().color = startColor;

                        startSquare = hit.transform.gameObject;
                        started = true;

                        startSquare.transform.GetComponent<SpriteRenderer>().color = selectedColor;
                    }
                    else
                    {
                        if (endSquare != null) endSquare.transform.GetComponent<SpriteRenderer>().color = startColor;

                        endSquare = hit.transform.gameObject;
                        started = false;

                        endSquare.transform.GetComponent<SpriteRenderer>().color = selectedColor;
                    }
                }
            }
        }
    }

    public void PlaceBridgeButton()
    {
        Vector3 spawnPos;
        //Check if both points share an axis, this makes sure bride is straight
        if (startSquare.transform.position.x == endSquare.transform.position.x || startSquare.transform.position.z == endSquare.transform.position.z)
        {
            //Tells code which direction to spawn bridge in
            bool moveOnX = (startSquare.transform.position.x == endSquare.transform.position.x) ? false : true;

            bool forward = false;
            if (moveOnX)
            {
                if (startSquare.transform.position.x > endSquare.transform.position.x) forward = true;
            }
            else
            {
                if (startSquare.transform.position.z > endSquare.transform.position.z) forward = true;
            }

            float distance = Vector3.Distance(startSquare.transform.position, endSquare.transform.position);
            print(distance);

            int direction = (forward) ? -1 : 1;

            if (distance < maxBridgeLength)
            {
                for (int i = 0; i < distance; i++)
                {
                    //Add one to x, else add one to Z
                    //ADD CONDITION TO WORK FOR NEGITIVE DIRECTION
                    if (moveOnX) spawnPos = startSquare.transform.position + new Vector3(i * direction, 0, 0);
                    else spawnPos = startSquare.transform.position + new Vector3(0, 0, i * direction);

                    GameObject curBridgeCube = Instantiate(bridgeCube, spawnPos, Quaternion.identity);
                } 
            }
        }
    }
}
