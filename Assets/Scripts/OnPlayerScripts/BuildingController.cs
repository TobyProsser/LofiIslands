using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingController : MonoBehaviour
{
    public bool canBuild = true;

    public AllBuildings allBuildings;

    public GameObject selectedBuilding;
    Building curBuilding;

    public GameObject testPlacementObject;
    GameObject curTestPlacementObject;

    Vector3 offset;

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
                    if (curTestPlacementObject != null) Destroy(curTestPlacementObject);

                    offset = Vector3.zero;

                    //if size of building is odd, make offset
                    if (curBuilding.size.x % 2 != 0)
                    {
                        offset.x = curBuilding.size.x / 2;
                    }

                    if (curBuilding.size.z % 2 != 0)
                    {
                        offset.z = curBuilding.size.z / 2;
                    }

                    Vector3 adjustedPos = new Vector3(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y) + 0.5f, Mathf.Floor(hit.point.z)) + offset;
                    curTestPlacementObject = Instantiate(testPlacementObject, adjustedPos, Quaternion.identity);

                    curTestPlacementObject.transform.localScale = curBuilding.size;
                    //If curBuilding is a water building, set testplacement object's sphere radius to water radius size
                    if (curBuilding.waterBuilding)
                    {
                        float radius = curBuilding.buildingObject.GetComponent<SphereCollider>().radius;
                        curTestPlacementObject.transform.GetChild(0).transform.localScale = new Vector3(radius/2, radius / 2, 1);
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && canBuild && Input.touchCount != 2)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "GridSquare")
                {
                    Vector3 location = new Vector3(Mathf.Floor(hit.point.x), Mathf.Floor(hit.point.y) + 0.5f, Mathf.Floor(hit.point.z)) + offset;
                    if (curTestPlacementObject) curTestPlacementObject.transform.position = location;
                }
            }
        }
    }

    public void BuildButton()
    {
        Vector3 spawnPos = curTestPlacementObject.transform.position;
        Destroy(curTestPlacementObject);

        GameObject placedBuilding = Instantiate(curBuilding.buildingObject, spawnPos, Quaternion.identity);
        placedBuilding.transform.parent = ChangingIslandsController.currentIsland.transform;
    }

    public void GetCurBuilding(int buildingNum)
    {
        curBuilding = allBuildings.allBuildings[buildingNum];
    }
}
