using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimationHandler : MonoBehaviour
{
    public float walkAnimationSpeed;
    public Mesh[] walkMeshes = new Mesh[4];
    public Mesh idleMesh;

    MeshFilter meshFilter;

    public bool idle;
    public bool walking;

    NavMeshAgent agent;

    private void Awake()
    {
        agent = this.transform.parent.GetComponent<NavMeshAgent>();
        meshFilter = this.GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if (agent.velocity.x != 0 && !walking || agent.velocity.z != 0 && !walking)
        {
            walking = true;
            idle = false;
            StartCoroutine(WalkCycle());
        }
        else if(agent.velocity.x == 0 && agent.velocity.z == 0 && !idle)
        {
            walking = false;
            idle = true;
            IdleCycle();
        }
    }
    //Loop through walk meshes and set them to the mesh filter
    //Once loop is done, reset loop
    public IEnumerator WalkCycle()
    {
        int i = 0;
        while (walking)
        {
            meshFilter.mesh = walkMeshes[i];
            yield return new WaitForSeconds(walkAnimationSpeed);
            i++;
            if (i >= walkMeshes.Length) i = 0;
        }
    }

    void IdleCycle()
    {
        meshFilter.mesh = idleMesh;
    }
}
