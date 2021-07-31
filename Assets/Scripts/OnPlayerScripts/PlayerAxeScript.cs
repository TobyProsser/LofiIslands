using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAxeScript : MonoBehaviour
{
    public bool canAxe;

    NavMeshAgent agent;

    List<GameObject> selectedTrees = new List<GameObject>();
    bool cuttingTrees;

    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAxe && Input.touchCount != 2)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Tree")
                {
                    print("Hit Tree");
                    GameObject curTreeObject = hit.transform.gameObject;
                    //if plant is not already in list, add it
                    if (!selectedTrees.Contains(curTreeObject)) selectedTrees.Add(curTreeObject);

                    //if not already harvesting, run coroutine,
                    //else do not run it
                    if (!cuttingTrees)
                    {
                        StartCoroutine(WalkToAndDestroyTree());
                        cuttingTrees = true;
                    }
                }
            }
        }
    }

    IEnumerator WalkToAndDestroyTree()
    {
        GameObject curTree = selectedTrees[0];
        int i = 0;
        while (selectedTrees.Count > i)
        {
            //get next tree in list
            curTree = selectedTrees[i];
            //Set agents destination to current tree
            if (curTree)
            {
                agent.SetDestination(curTree.transform.position);
                //Wait for agent to get to tree
                while (Mathf.Abs(Vector3.Distance(agent.transform.position, curTree.transform.position)) > 3f)
                {
                    //print(Mathf.Abs(Vector3.Distance(agent.transform.position, curTree.transform.position)));
                    yield return null;
                }

                Destroy(curTree);
            }

            if (selectedTrees.Count < i) break;

            i++;
            yield return null;
        }

        cuttingTrees = false;
        print("Trees Cut");
    }
}
