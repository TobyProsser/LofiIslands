using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnIslandButton : MonoBehaviour
{
    public ChangingIslandsController changingIslandsController;

    public int islandNumber;

    void Awake()
    {
        //subscribe to the onClick event
        this.GetComponent<Button>().onClick.AddListener(CustomButton_onClick);
    }

    //Handle the onClick event
    void CustomButton_onClick()
    {
        changingIslandsController.ChangeIsland(islandNumber);
    }
}
