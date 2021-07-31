using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovementController : MonoBehaviour
{
    float speed;

    void Start()
    {
        speed = PlatformsSpawner.platformSpeed;
    }

    void LateUpdate()
    {
        this.transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
    }
}
