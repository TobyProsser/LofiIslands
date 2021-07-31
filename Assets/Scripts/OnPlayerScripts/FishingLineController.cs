using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class FishingLineController : MonoBehaviour
{
    public bool canFish = true;

    [Header("Fishing Rod Attributes")]
    public float rarityChance;
    public float minTime;
    public float maxTime;
    public float health;

    public float fishingDistance = 40;

    [Header("Extras")]
    public LineRenderer fishingLine;

    bool fishing;

    public GameObject lineEndObject;
    GameObject curLineEndObject;

    public Transform startPos;
    Vector3 endPos;

    public Camera camera;
    CameraController camController;

    NavMeshAgent agent;

    public GameObject fishingGamePanel;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        camController = camera.GetComponent<CameraController>();
    }

    void Update()
    {
        if (canFish && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && Input.touchCount != 2)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Water")
                {

                    if (!fishing)
                    {
                        //store click point
                        endPos = hit.point;
                        //Destory endObject if it exists
                        if (curLineEndObject) Destroy(curLineEndObject);
                        //get destination for player on shore
                        Vector3 destination = FindClosestDestination(50, endPos);
                        //if destination was found
                        if (destination != Vector3.positiveInfinity)
                        {
                            //set agent destination
                            agent.SetDestination(destination);
                            //start walk 'timer'
                            StartCoroutine(CheckForPosition(destination));
                        }
                    }
                    else
                    {
                        //destory endobject and fishing line if player clicks
                        //while already fishing
                        fishing = false;
                        if (curLineEndObject) Destroy(curLineEndObject);
                        fishingLine.SetVertexCount(0);
                    }
                }//if player clicks on grass, stop fishing
                else if (hit.transform.tag == "Grass")
                {
                    fishing = false;

                    if (curLineEndObject) Destroy(curLineEndObject);

                    fishingLine.SetVertexCount(0);
                }
            }
            
        }

        //while fishing, make sure line is attached to fishing rod
        if (fishing)
        {
            fishingLine.SetPosition(0, startPos.position);
        }
    }

    //wait till player is in position
    IEnumerator CheckForPosition(Vector3 destination)
    {
        //checks if this courotine runs for too long
        float counter = 0;

        while (true)
        {
            if (Mathf.Abs(Vector3.Distance(this.transform.position, destination)) < 1.5f) break;

            counter++;
            if (counter >= 3000) break;

            yield return null;
        }

        if(counter < 1000) CreateFishingLine(endPos);
    }

    void CreateFishingLine(Vector3 location)
    {
        fishingLine.SetVertexCount(2);
        fishingLine.SetPosition(0, startPos.position);
        fishingLine.SetPosition(1, endPos);
        fishingLine.enabled = true;
        curLineEndObject = Instantiate(lineEndObject, location, Quaternion.identity);

        fishing = true;

        StopAllCoroutines();

        StartCoroutine(FindFish());
    }

    Vector3 GetCurrentMousePosition()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Water")
            { 
                return hit.point;
            }
            else if (endPos != null) return endPos;
            return Vector3.positiveInfinity;
        }
        else return Vector3.positiveInfinity;
    }

    //after clicking on water, find closest point on island for player to walk to
    //to fish from
    Vector3 FindClosestDestination(int range, Vector3 location)
    {
        List<Vector3> points = new List<Vector3>();
        //Get all points in area
        for (int x = Mathf.RoundToInt(location.x) - range; x < Mathf.RoundToInt(location.x) + range; x++)
        {
            for (int z = Mathf.RoundToInt(location.x) - range; z < Mathf.RoundToInt(location.x) + range; z++)
            {
                //get y value of player to find the height of the navMesh
                Vector3 testPoint = new Vector3(x, this.transform.position.y, z);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(testPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    Vector3 result = hit.position;

                    points.Add(result);
                }
            }
        }

        float distance = Mathf.Infinity;
        Vector3 shortest = Vector3.positiveInfinity;
        //get closest point
        foreach (Vector3 point in points)
        {
            if (Mathf.Abs(Vector3.Distance(point, location)) < distance)
            {
                distance = Mathf.Abs(Vector3.Distance(point, location));
                shortest = point;
            }
        }

        return shortest;
    }

    //Called by FishingMiniGameController to 
    //RestartFindFish
    public void StartFindFish()
    {
        StartCoroutine(FindFish());
    }

    IEnumerator FindFish()
    {
        //Randomly salect time for next fish to spawn
        float waitTime = Random.Range(minTime, maxTime);
        print(waitTime);
        yield return new WaitForSeconds(waitTime);

        if (fishing)
        {
            fishingGamePanel.SetActive(true);
            print("Fishing: " + fishing);
        }
    }
}
