using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigameController : MonoBehaviour
{
    Fish curFish;
    float fishSpeed;
    float catchSpeed;

    public AllFish allFishScript;

    public GameObject fishSprite;   //Black shadow of fish, can't tell what fish it is until it's caught
    public GameObject pointerSprite;

    public Slider progressSlider;
    public float progress = 30;

    bool caught;
    bool endConditionMet;

    Image gamePanel;
    float sizeX;
    float sizeY;

    public GameObject fishCaughtPanel;
    public GameObject fishLostPanel;
    public FishingLineController fishingLineController;

    private void Awake()
    {
        gamePanel = this.GetComponent<Image>();

        //FIX FOR DIFFERENT SCREEN SIZES
        sizeX = gamePanel.transform.localScale.x;
        sizeY = gamePanel.transform.localScale.y;

        //make sure this panel isn't active when game starts
        //this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        //SETUP
        caught = false;
        endConditionMet = false;

        fishCaughtPanel.SetActive(false);
        fishLostPanel.SetActive(false);

        pointerSprite.SetActive(true);
        fishSprite.SetActive(true);

        //randomly select fish
        curFish = allFishScript.allFish[Random.Range(0, allFishScript.allFish.Count)];
        //fishSpeed = curFish.fishSpeed;
        //fishSpeed = curFish.catchSpeed;
        fishSpeed = 6;
        catchSpeed = .3f;

        progress = 30;
        StartCoroutine(MoveFish());
    }

    void FixedUpdate()
    {
        progressSlider.value = progress;

        //if pointer and fish sprite are close together, add to progress slider
        if (Mathf.Abs(Vector3.Distance(fishSprite.transform.position, pointerSprite.transform.position)) < .5f)
        {
            progress += catchSpeed;
        } //else substract from progress
        else if(!caught) progress -= catchSpeed;

        //if Progress is over 100, run endCondition
        if (progress >= 100 && !endConditionMet) 
        {
            WinCondition();
            caught = true;
            pointerSprite.SetActive(false);
        }
        else if (progress <= 0 && !endConditionMet) LoseCondition();

        //if mouse button, and progress is left, make pointer follow mouse
        if (Input.GetMouseButton(0) && progress > 0)
        {
            var screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f; //distance of the plane from the camera
            pointerSprite.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        }
        else if (progress < 0)
        {
            pointerSprite.SetActive(false);
        }

        //If end condition has been met, and player clicks
        //Set this gameobject to unactive
        if (Input.GetMouseButtonDown(0) && endConditionMet)
        {
            //Run timer to allow player to continue finding fish
            fishingLineController.StartFindFish();

            this.gameObject.SetActive(false);
        }
    }

    IEnumerator MoveFish()
    {
        //Find point within screen
        Vector3 moveToLoc = new Vector3(Random.Range(-400, 400), Random.Range(-800, 800), 0);

        while (progress > 0 && !caught)
        {
            //Move fish to point
            fishSprite.transform.localPosition = Vector3.MoveTowards(fishSprite.transform.localPosition, moveToLoc, fishSpeed * Time.deltaTime * 10);

            //Once fish has reached point, set a new point
            if(Mathf.Abs(Vector3.Distance(moveToLoc, fishSprite.transform.localPosition)) < .2f) moveToLoc = new Vector3(Random.Range(-400, 400), Random.Range(-800, 800), 0);
            
            //print(Mathf.Abs(Vector3.Distance(moveToLoc, fishSprite.transform.localPosition)) + " " + fishSprite.transform.localPosition + " " + moveToLoc);

            yield return null;
        }

        //Hide fish after timer runs out, or it is caught
        fishSprite.SetActive(false);
    }

    void WinCondition()
    {
        //ADD FISH TO INVENTORY
        endConditionMet = true;
        fishCaughtPanel.SetActive(true);
    }

    void LoseCondition()
    {
        endConditionMet = true;
        fishLostPanel.SetActive(true);
    }

}
