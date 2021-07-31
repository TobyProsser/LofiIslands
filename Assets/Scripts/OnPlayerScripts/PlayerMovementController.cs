using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{
    public bool canWalk;

    NavMeshAgent agent;

    public GameObject storePanel;
    public GameObject sellingPanel;

    bool clickedSellingShop;
    bool clickedStore;
    bool clickedDock;
    Vector3 destination;

    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && Input.touchCount != 2 && canWalk)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Grass")
                {
                    agent.SetDestination(hit.point);
                }
                else if (hit.transform.tag == "Dock")
                {
                    agent.SetDestination(hit.point);
                    clickedDock = true;
                    destination = hit.transform.position;
                }
                else if (hit.transform.tag == "Store")
                {
                    agent.SetDestination(hit.point);
                    clickedStore = true;
                    destination = hit.transform.position;
                }
                else if (hit.transform.tag == "SellingStore")
                {
                    agent.SetDestination(hit.point);
                    clickedSellingShop = true;
                    destination = hit.transform.position;
                }
            }
        }

        if (clickedDock)
        {
            if (Mathf.Abs(Vector3.Distance(this.transform.position, destination)) <= 10)
            {
                Camera.main.GetComponent<CameraController>().MoveCamToBoat(destination);
                clickedDock = false;
            }
        }

        if (clickedStore)
        {
            if (Mathf.Abs(Vector3.Distance(this.transform.position, destination)) <= 5)
            {
                storePanel.SetActive(true);
                clickedStore = false;
            }
        }

        if (clickedSellingShop)
        {
            if (Mathf.Abs(Vector3.Distance(this.transform.position, destination)) <= 5)
            {
                sellingPanel.SetActive(true);
                clickedSellingShop = false;
            }
        }
    }
}
