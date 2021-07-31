using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionsPanelController : MonoBehaviour
{
    public GameObject actionButtons;
    public GameObject showActionButton;

    public GameObject player;

    public NavMeshSurface surface;

    GameObject grid = null;

    GameObject camera;
    public GameObject plantsPanel;
    public GameObject buildingsPanel;
    public GameObject harvestPanel;
    public GameObject bridgePanel;

    private void Awake()
    {
        camera = Camera.main.gameObject;

        actionButtons.SetActive(false);
        showActionButton.SetActive(true);

        DisableAllActions();


        player.GetComponent<PlayerMovementController>().canWalk = true;

        plantsPanel.SetActive(false);
        buildingsPanel.SetActive(false);
        harvestPanel.SetActive(false);
        bridgePanel.SetActive(false);
    }

    public void ShowActionButtons()
    {
        actionButtons.SetActive(true);
        showActionButton.SetActive(false);
    }

    void HideActionButtons()
    {
        actionButtons.SetActive(false);
        showActionButton.SetActive(true);
    }

    void DisableAllActions()
    {
        player.GetComponent<PlayerPlantingScript>().canPlant = false;
        player.GetComponent<FishingLineController>().canFish = false;
        player.GetComponent<PlayerAxeScript>().canAxe = false;
        player.GetComponent<PlayerHarvestScript>().canHarvest = false;
        player.GetComponent<PlayerMovementController>().canWalk = false;
        player.GetComponent<BuildingController>().canBuild = false;
    }

    public void AxeButton()
    {
        DisableAllActions();

        player.GetComponent<PlayerAxeScript>().canAxe = true;
        HideActionButtons();
    }

    public void ScytheButton()
    {
        DisableAllActions();

        showActionButton.SetActive(false);

        player.GetComponent<PlayerHarvestScript>().canHarvest = true;
        HideActionButtons();

        harvestPanel.SetActive(true);
        TopDown();
    }

    public void HoeButton()
    {
        DisableAllActions();

        showActionButton.SetActive(false);

        player.GetComponent<PlayerPlantingScript>().canPlant = true;
        HideActionButtons();

        plantsPanel.SetActive(true);
        TopDown();
    }

    public void FishingButton()
    {
        DisableAllActions();

        player.GetComponent<FishingLineController>().canFish = true;
        HideActionButtons();
    }

    public void WalkButton()
    {
        DisableAllActions();

        player.GetComponent<PlayerMovementController>().canWalk = true;
        HideActionButtons();
    }

    public void BuildButton()
    {
        DisableAllActions();

        showActionButton.SetActive(false);

        player.GetComponent<BuildingController>().canBuild = true;
        HideActionButtons();

        buildingsPanel.SetActive(true);
        TopDown();
    }

    public void BridgeButton()
    {
        DisableAllActions();

        showActionButton.SetActive(false);

        player.GetComponent<BridgeCreator>().canBuild = true;
        HideActionButtons();

        bridgePanel.SetActive(true);
        TopDown();
    }

    void TopDown()
    {
        camera.GetComponent<CameraController>().RunTopDown();

        //Find grid object
        GameObject island = GameObject.FindGameObjectWithTag("Island");
        foreach (Transform child in island.transform)
        {
            if (child.tag == "GridObject") grid = child.gameObject;
        }

        grid.SetActive(true);
    }

    public void BackButton()
    {
        grid.SetActive(false);

        plantsPanel.SetActive(false);
        buildingsPanel.SetActive(false);
        harvestPanel.SetActive(false);
        bridgePanel.SetActive(false);
        camera.GetComponent<CameraController>().ResetCamera();

        showActionButton.SetActive(true);

        WalkButton();
    }
}
