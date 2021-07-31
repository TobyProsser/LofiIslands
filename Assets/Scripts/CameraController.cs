using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    Camera camera;
    public GameObject player;

    public GameObject water;
    Vector3 startWaterSize;
    public Vector3 zoomedWaterSize;

    Quaternion startLookRot;
    Vector3 startPos;

    float startFieldOfView = 60;

    public float zoomSpeed;
    public float lookSpeed;
    public float moveSpeed;

    Vector3 targetPos;

    public Vector3 dockOffset;
    public float boatCamSpeed;

    //given by FishingLineController
    [HideInInspector]
    public bool stopZoom = false;

    bool resetingCamera;
    bool topDown;

    public GameObject boatPanel;

    PanZoom panZoom;

    void Start()
    {
        camera = this.transform.GetComponent<Camera>();
        panZoom = this.GetComponent<PanZoom>();

        startLookRot = this.transform.rotation;
        startPos = this.transform.position;
        startWaterSize = water.transform.localScale;

        startFieldOfView = camera.fieldOfView;

        boatPanel.SetActive(false);
    }

    //Run by FishingLineController
    public void RunTopDown()
    {
        transform.Rotate(45, 0, 0);
        transform.position = new Vector3(0, 70, 0);
    }

    public void ResetCamera()
    {
        StopAllCoroutines();
        topDown = false;
        this.transform.position = startPos;
        StartCoroutine(ResetCamRot());
    }

    IEnumerator ResetCamRot()
    {
        resetingCamera = true;
        StartCoroutine(CamZoom(startFieldOfView));
        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startLookRot, lookSpeed * Time.deltaTime);

            if (transform.rotation == startLookRot) break;
            yield return null;
        }

        resetingCamera = false;
    }

    IEnumerator CamZoom(float FOV)
    {
        print(camera.fieldOfView);
        float tempFOV = camera.fieldOfView;
        while (tempFOV != FOV)
        {
            float tempSpeed = zoomSpeed * Time.deltaTime;
            tempFOV = Mathf.Lerp(tempFOV, FOV, tempSpeed);
            camera.fieldOfView = tempFOV;

            if (Mathf.Abs(tempFOV - FOV) < .3f) break;
            yield return null;
        }
    }

    public void MoveCamToBoat(Vector3 loc)
    {
        panZoom.canMoveCamera = false;
        StartCoroutine(BoatZoomInCamera(loc));
    }

    public void MoveBoatCamBack()
    {
        panZoom.canMoveCamera = true;
        StartCoroutine(BoatZoomOutCamera());
    }

    IEnumerator BoatZoomInCamera(Vector3 dockLoc)
    {
        Vector3 target = dockLoc + dockOffset;
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * boatCamSpeed);
            water.transform.localScale = Vector3.Lerp(water.transform.localScale, zoomedWaterSize, Time.deltaTime * boatCamSpeed);

            if (Mathf.Abs(Vector3.Distance(transform.position, target)) < .1f)
            {
                transform.position = target;
                break;
            }
            yield return null;
        }

        boatPanel.SetActive(true);
    }

    IEnumerator BoatZoomOutCamera()
    {
        boatPanel.SetActive(false);

        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * boatCamSpeed);
            water.transform.localScale = Vector3.Lerp(water.transform.localScale, startWaterSize, Time.deltaTime * boatCamSpeed);

            if (Mathf.Abs(Vector3.Distance(transform.position, startPos)) < .1f)
            {
                transform.position = startPos;
                break;
            }
            yield return null;
        }
    }
}
